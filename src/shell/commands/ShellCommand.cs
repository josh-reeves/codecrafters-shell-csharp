using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Interfaces;

namespace Shell.Commands;


/* [DESIGN NOTE] Typing up the below comment and considering the following:
 *  "This includes the creation of any required shell forks...." On the one
 *  hand, the shell should probably be responsible for forking itself (and maybe
 *  even setting up its pipes). It makes sense, and they add a lot of complexity 
 *  to this class that could stand to be offloaded elsewhere. On the other hand,
 *  the benefit of handling those functions here, is that this class already 
 *  includes logic for executing external commands (for forking), and must be
 *  able to identify and handle redirection nodes, which is tangentially
 *  related.
 *
 *  This isn't a big deal right now, and may even be considered beneficial by
 *   some (deep class vs. shallow class). It may be something to consider in the
 *   future, though.*/

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

            AnonymousPipeServerStream stream = new(
                PipeDirection.Out, 
                HandleInheritability.Inheritable);

            Shell.OutWriters.Add(new StreamWriter(stream) {AutoFlush = true});

            ExecuteExternal(new ProcessStartInfo("codecrafters-shell",
                [ RestoreCommand(child),
                  "-i",
                  stream.GetClientHandleAsString() ])); 

            stream.DisposeLocalCopyOfClientHandle();

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
    /// 
    private void ExecuteExternal(ProcessStartInfo startInfo)
    {
        Process process = new() { StartInfo = startInfo};

        process.Start();

        Shell.Forks.Add(process);

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
            while (Shell.InReader?.ReadLine() is string input)
            {
                process.StandardInput.WriteLine(input);
#if DEBUG
                Console.WriteLine($"[DEBUG] Writing to stdin of {process.StartInfo.FileName}: {input}");
#endif
            }

            process.StandardInput.Close();

        }

    }

    #endregion

}