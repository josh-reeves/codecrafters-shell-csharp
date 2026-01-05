using Interfaces;

namespace Shell.Commands;

public abstract class ShellCommand : IShellCommand
{
    public ShellCommand(IShell shell)
    {
        Shell = shell;

    }

    protected IShell Shell { get; set; }

    public abstract void Execute(object[]? args);   

}