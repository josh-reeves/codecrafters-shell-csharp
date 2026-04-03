using Interfaces;

namespace Shell.Commands;

public class Exit : ShellCommand
{
    #region Constructors(s)
    public Exit(IShell shell) : base(shell) {}

    #endregion

    #region Methods
    public override void Execute(object? args = null)
    {
        Shell.ShellIsActive = false;

    }

    #endregion

}