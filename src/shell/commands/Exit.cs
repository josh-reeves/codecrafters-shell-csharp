using Interfaces;

namespace Shell.Commands;

public class Exit : ShellCommand
{
    public Exit(IShellEnvironment environment) : base(environment) {}

    public override void Execute(object[]? args)
    {
        Environment.ShellIsActive = false;

    }

}