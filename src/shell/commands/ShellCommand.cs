using Interfaces;

namespace Shell.Commands;

public abstract class ShellCommand : IShellCommand
{
    private string standardOutput,
                   standardError;

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

    public abstract void Execute(object[]? args);   

}