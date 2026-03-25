using Interfaces;

namespace Shell.Commands;

public class PrintWorkingDirectory : ShellCommand
{
    #region Constructor(s)
    public PrintWorkingDirectory(IShell shell) : base(shell) {}
    
    #endregion

    #region Methods
    public override void Execute(object? args = null)
    {
        StandardOutput += Directory.GetCurrentDirectory();

        if (IsStdOutRedirected)
        {
            return;

        }
        
        Console.WriteLine(StandardOutput);

    }

    #endregion

}