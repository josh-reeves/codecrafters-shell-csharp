using System.Diagnostics;
using System.IO.Pipes;
using Interfaces;

namespace Shell.Commands;

public class ShellCommand : IShellCommand
{
    #region Fields
    private string command,
                   standardOutput,
                   standardError;

    private IList<string> arguments;

    #endregion
    
    #region Constructor(s)
    public ShellCommand(IShell shell)
    {
        command = string.Empty;
        standardOutput = string.Empty;
        standardError = string.Empty;
        StandardInput = string.Empty;

        arguments = new List<string>();

        InvalidCmdMsg = ": command not found";

        Shell = shell;

    }

    #endregion

    #region Properties
    protected IShell Shell { get; set; }

    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public string InvalidCmdMsg { get; set; }

    public string StandardInput { get; private set; }

    public string StandardOutput
    {
        get
        {
            string result = standardOutput;

            standardOutput = string.Empty;

            return result;

        }

        set => standardOutput = value;
    }

    public string StandardError
    {
        get
        {
            string result = standardError;

            standardError = string.Empty;

            return result;
            
        }

        set => standardError = value;

    }

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
            IShellCommand builtin = Shell.Builtins[command];

            builtin.IsStdOutRedirected = IsStdOutRedirected;
            builtin.IsStdErrRedirected = IsStdErrRedirected;
            
            Shell.Builtins[command].Execute(arguments.ToArray());

            StandardOutput = Shell.Builtins[command].StandardOutput;
            StandardError = Shell.Builtins[command].StandardError;

            return;
            
        }

        if (Shell.IsExecutable([..Shell.Search(command, Shell.PathList)]))
        {
            ExecuteExternal(command, [..arguments]);

            return;
            
        }

        Console.WriteLine(command + InvalidCmdMsg);
        
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
        Stream stream;

        if (node.Data.Type is TokenType.Pipe)
        {
            stream = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

            Shell.OutWriters.Add(new StreamWriter(stream) { AutoFlush = true});
            
            IsStdOutRedirected = true;

            new ShellCommand(this.Shell)
            {
                StandardInput = ((AnonymousPipeServerStream)stream).GetClientHandleAsString()

            }.Execute(node.RightChild);

            return;

        }

        if (node is IOutputToFileNode output)
        {
            stream = new FileStream(output.FileToken.ExpandedValue, output.FileMode, FileAccess.Write);

            RedirectToFile(stream, node.Data.Type);

        }

    }

    private void RedirectToFile(Stream stream, TokenType tokenType)
    {
        if (tokenType is TokenType.RedirectStdOut or TokenType.AppendStdOut)
        {
            Shell.OutWriters.Add(new StreamWriter(stream) {AutoFlush = true});

            IsStdOutRedirected = true;
           
        }

        if (tokenType is TokenType.RedirectStdErr or TokenType.AppendStdErr)
        {
            Shell.ErrWriters.Add(new StreamWriter(stream) {AutoFlush = true });

            IsStdErrRedirected = true;
            
        }

    }


    /// <summary>
    /// Execute a command that isn't built into the shell.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="args">The arguments to pass to the command.</param>
    private void ExecuteExternal(string command, string[] args)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = command,
                UseShellExecute = !IsStdOutRedirected && !IsStdErrRedirected,
                RedirectStandardOutput = IsStdOutRedirected,
                RedirectStandardError = IsStdErrRedirected

            }
            
        };

        if (!string.IsNullOrWhiteSpace(StandardInput))
        {
            process.StartInfo.ArgumentList.Add(StandardInput);

        }

        foreach (string arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);

        }

        process.Start();

        if (IsStdOutRedirected)
        {
            StandardOutput = process.StandardOutput.ReadToEnd();

        }

        if (IsStdErrRedirected)
        {
            StandardError = process.StandardError.ReadToEnd();
            
        }

        process.WaitForExit();

    }

    #endregion

}