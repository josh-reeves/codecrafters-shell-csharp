using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Interfaces;

namespace Shell.Core.Commands;

/// <summary>
///  The ShellCommand class handles the interpretation and execution of parsed 
///   shell input. This includes the creation of any required shell forks and
///   their associated pipes, as well as creating any required input and output 
///   streams. 
/// 
///  The way the class does this is somewhat recursive in nature:
///   1. When the class's Execute method is passed an IShellNode, it begins
///       walking, interpreting and executing the tree that propagates from that
///       node. 
///   2. The class also acts as a base class for the various shell built-ins, 
///       where its Execute method typically takes a list of optional arguments 
///       to pass to the built-in.
/// 
///  So the class will take a command tree, interpret it and, if that command
///   tree represents a built-in, create a new instance of itself to handle the
///   execution of the built-in's logic.
/// </summary>
public class ShellCommand : IShellCommand
{
    #region Fields
    private string command;

    private IList<string> arguments;

    #endregion
    
    #region Constructor(s)
    public ShellCommand(IShell shell)
    {
        command = string.Empty;

        arguments = new List<string>();

        InvalidCmdMsg = ": command not found";

        StandardOutput = StreamReader.Null;
        StandardError = StreamReader.Null;

        Shell = shell;

    }

    #endregion

    #region Properties
    protected IShell Shell { get; set; }

    public bool IsStdInRedirected { get => !(Shell.InReader == null); }

    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public string InvalidCmdMsg { get; set; }

    public StreamReader StandardOutput { get; internal set; }

    public StreamReader StandardError { get; internal set; }

    public IDebugger? Debugger { get; set; }

    #endregion

    #region Methods
    public virtual void Execute(object? args)
    {
        if (args is not IShellNode node)
        {
            return;
            
        }

        Stack<ITreeNode> nodes = new();

        nodes.Push(node);

        while (nodes.Count > 0)
        {
            node = (IShellNode)nodes.Pop();

            ProcessNode(node);

            if (node.RightChild is not null)
            {
                nodes.Push(node.RightChild);

            }

            if (node.LeftChild is not null)
            {
                nodes.Push(node.LeftChild);

            }

        }
#if DEBUG
        IList<string> arglist = new List<string>();

        // Need to move this into the debugger class and make it reusable:
        foreach (string arg in arguments)
        {
            switch (arg)
            {
                case "":
                    arglist.Add("<empty string>");

                    continue;

                case " ":
                    arglist.Add("<space>");

                    continue;

                case "\n":
                    arglist.Add("<newline>");

                    continue;

                default:
                    arglist.Add(arg);

                    continue;

            }
            
        }

        Debugger?.WriteLine($"EXECUTION: Executing command {command}; IsInputRedirected: {IsStdInRedirected}; Arguments: [{string.Join(", ", arglist)}]");
#endif

        if (Shell.Builtins.ContainsKey(command))
        {
            IShellCommand builtin = Shell.Builtins[command].Invoke();

            builtin.IsStdOutRedirected = IsStdOutRedirected;
            builtin.IsStdErrRedirected = IsStdErrRedirected;
            
            builtin.Execute(arguments.ToArray());

            StandardOutput = builtin.StandardOutput;
            StandardError = builtin.StandardError;

            return;   

        }

        if (Shell.IsExecutable([..Shell.Search(command, Shell.PathList)]))
        {
            ExecuteExternal(new ProcessStartInfo(command, arguments)
            {
                UseShellExecute = 
                    !IsStdInRedirected &&
                    !IsStdOutRedirected &&
                    !IsStdErrRedirected,
                RedirectStandardOutput = IsStdOutRedirected,
                RedirectStandardError = IsStdErrRedirected,
                RedirectStandardInput = IsStdInRedirected,

            });

            return;
            
        }

        Console.WriteLine(command + InvalidCmdMsg);
        
    }

    internal StreamReader StreamReaderFromString(string input)
    {
        MemoryStream stream = new(Encoding.UTF8.GetBytes(input));
        StreamReader reader = new(stream);

        return reader;
        
    }

    /// <summary>
    ///     Restores a command string from the supplied node.
    /// </summary>
    /// <param name="node">
    ///     The node from which to restore the command string.
    /// </param>
    /// <returns>
    ///     A string containing the original command used to create the node.
    /// </returns>
    private string RestoreCommand(IShellNode node)
    {
        string command = string.Empty;

        Stack<ITreeNode> nodes = new();

        nodes.Push(node);

        while (nodes.Count > 0)
        {
            node = (IShellNode)nodes.Pop();

            command += node.Data.RawValue + Shell.CommandSeparator;

            if (node.RightChild is not null)
            {
                nodes.Push(node.RightChild);

            }

            if (node.LeftChild is not null)
            {
                nodes.Push(node.LeftChild);

            }
               
        }      

        return command;  

    }

    private void ProcessNode(IShellNode node)
    {        
        switch(node.NodeType)
        {
            case NodeType.Command:
                command = node.Data.ExpandedValue;

                break;

            case NodeType.Argument:
                arguments.Add(node.Data.ExpandedValue);

                break;

            case NodeType.OutputRedirection:
                Redirect(node);

                break;

        }
        
    }

    private void Redirect(IShellNode node)
    {
        if (node is IOutputToFileNode output)
        {
            RedirectToFile(output);

        }

        if (node.Data.Type is TokenType.Pipe)
        {
            RedirectToPipe(node);

        }
        
    }

    private void RedirectToFile(IOutputToFileNode node)
    {
        FileStream stream = new(node.FileToken.ExpandedValue, node.FileMode, FileAccess.Write);

        if (node.Data.Type is TokenType.RedirectStdOut or TokenType.AppendStdOut)
        {
            Shell.OutWriters.Add(new StreamWriter(stream) {AutoFlush = true});

            IsStdOutRedirected = true;
           
        }

        if (node.Data.Type is TokenType.RedirectStdErr or TokenType.AppendStdErr)
        {
            Shell.ErrWriters.Add(new StreamWriter(stream) {AutoFlush = true });

            IsStdErrRedirected = true;
            
        }

    }

    private void RedirectToPipe(IShellNode node)
    {
        try
        {
            if (node.LeftChild is not IShellNode child)
            {
                return;
                
            }  

            IsStdOutRedirected = true;

            node.RemoveChild(child);
            
            Debugger?.WriteLine($"EXECUTION: Child node removed: {RestoreCommand(child)}");
            
            AnonymousPipeServerStream stream = new(
                PipeDirection.Out, 
                HandleInheritability.Inheritable);

            Shell.OutWriters.Add(new StreamWriter(stream) {AutoFlush = true});

            ExecuteExternal(new ProcessStartInfo("codecrafters-shell",
                [ RestoreCommand(child),
                  "-i",
                  stream.GetClientHandleAsString() ])); 

            stream.DisposeLocalCopyOfClientHandle();
            
            Debugger?.WriteLine($"EXECUTION: Local copy of client handle removed.");
        
        }
        catch (Exception ex)
        {
            Trace.WriteLine(ex);

        }

    }

    /// <summary>
    ///  Execute a command that isn't built into the shell.
    /// </summary>
    /// <param name="startInfo">
    /// </param>
    private void ExecuteExternal(ProcessStartInfo startInfo)
    {
        Process process = new() { StartInfo = startInfo};

        process.Start();

        Shell.Forks.Add(process);

        Debugger?.WriteLine($"EXECUTION: {process.StartInfo.FileName} added to shell forks");

        if (process.StartInfo.RedirectStandardOutput)
        {
            StandardOutput = process.StandardOutput;

        }

        if (process.StartInfo.RedirectStandardError)
        {
            StandardError = process.StandardError;

        }

        if (process.StartInfo.RedirectStandardInput)
        {
            string? input = string.Empty;
            
            while ((input = Shell.InReader?.ReadLine()) is not null)
            {
                Debugger?.WriteLine($"EXECUTION: Writing to stdin of {process.StartInfo.FileName}: {input}");
                                
                try
                {
                    process.StandardInput.WriteLine(input);

                    Debugger?.WriteLine($"EXECUTION: {input} written to stdin of {process.StartInfo.FileName}");


                }
                catch (Exception ex)
                {
                    Debugger?.WriteLine($"EXECUTION: Failed to write {input} to stdin of {process.StartInfo.FileName}: {ex.Message}");                
                    
                    /* Presumably, stdin is no longer useable at this point,
                     *  so returning here prevents an additional exeption from
                     *  being thrown when we try to explicitly close it: */
                    return;

                    /* Worst case scenario, this will prevent all of the
                     *  expected data from being written to stdin, and since an
                     *  exception was thrown anyway, that's not entirely 
                     *  unexpected. Something to keep an eye on for edge cases,
                     *  maybe, but I think it's basically safe for now. */

                }


            }
            
            Debugger?.WriteLine($"EXECUTION: Attempting to manually close stdin of {process.StartInfo.FileName}.");

            process.StandardInput.Close();

            Debugger?.WriteLine($"EXECUTION: stdin of {process.StartInfo.FileName} manually closed after writing {input}");

        }

    }

    #endregion

}