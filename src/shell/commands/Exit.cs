using Interfaces;

namespace Shell.Commands;

public class Exit : ShellCommand
{
    public Exit(IShell shell) : base(shell) {}

    public override void Execute(object? args = null)
    {
        Shell.ShellIsActive = false;

    }

}