using Interfaces;

namespace Shell.Commands;

public class ChangeDirectory : ShellCommand
{
    #region Fields
    private const string invalidDirMsg = ": No such file or directory";

    #endregion

    #region Constructor(s)
    public ChangeDirectory(IShell shell) : base(shell) {}

    #endregion

    #region Methods
    public override void Execute(object? args)
    {
        string dir = (args as string[])?.Length > 0 ? ((string[])args)[0].Replace(Shell.HomeChar.ToString(), Shell.HomeDir) : string.Empty;
        
        if (string.IsNullOrEmpty(dir))
        {
            return;

        }

        if (Directory.Exists(dir))
        {
            Directory.SetCurrentDirectory(dir);

            return;

        }

        StandardError += dir + invalidDirMsg;

        if (IsStdErrRedirected)
        {
            return;

        }

        Console.WriteLine(StandardError);

    }

    #endregion

}