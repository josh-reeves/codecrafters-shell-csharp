using System.Diagnostics;
using Interfaces;
using Shell.Extensions.ShellInputHandler.Parser.Nodes;

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
        if (args is not ITree commandTree || commandTree.Root is null)
        {
            return;
            
        }

        Stack<ITreeNode> nodes = new();

        nodes.Push(commandTree.Root);

        while (nodes.Count > 0)
        {
            ProcessNode(nodes.Peek());

            foreach (ITreeNode node in nodes.Pop().Children.Reverse())
            {
                nodes.Push(node);
                
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

    private void ProcessNode(ITreeNode node)
    {
        switch(node)
        {
            case CommandNode commandNode:
                command = commandNode.Data.ExpandedValue;

                break;

            case ArgumentNode argumentNode:
                arguments.Add(argumentNode.Data.ExpandedValue);

                break;

            case RedirectorNode redirectorNode:
                Redirect(redirectorNode);

                break;
            
        }
        
    }

    private void Redirect(RedirectorNode node)
    {
        FileStream stream = new FileStream(node.FileToken.ExpandedValue, node.FileMode, FileAccess.Write);

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