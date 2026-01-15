using Interfaces;
using Shell.Extensions.ShellInputHandler.Parser.Nodes;

namespace Shell.Commands;

public abstract class ShellCommand : IShellCommand
{
    private string standardOutput,
                   standardError;

    private ITreeNode? iterator;

    public ShellCommand(IShell shell)
    {
        standardOutput = string.Empty;
        standardError = string.Empty;

        Shell = shell;

    }

    protected IShell Shell { get; set; }

    public string StandardOutput
    {
        get
        {
            string output = standardOutput;

            standardOutput = string.Empty;

            return output;

        }

        protected set
        {
            standardOutput = value;
            
        }

    }

    public string StandardError
    {
        get
        {
            string error = standardError;

            standardError = string.Empty;

            return error;

        }

        protected set
        {
            standardError = value;

        }
        
    }

    public virtual void Execute(object? args)
    {
        if (args is not CommandTree command)
        {
            return;
            
        }
        
    }

}