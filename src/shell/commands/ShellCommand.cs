using Shell.Extensions;
using Interfaces;

namespace Shell.Commands;

public abstract class ShellCommand : IShellCommand
{
    public ShellCommand(IShellEnvironment environment)
    {
        Environment = environment;

    }

    protected IShellEnvironment Environment { get; set; }

    public abstract void Execute(object[]? args);   

}