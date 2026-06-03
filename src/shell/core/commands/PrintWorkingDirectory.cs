using Interfaces;

namespace Shell.Core.Commands;

public class PrintWorkingDirectory : ShellCommand
{
    #region Constructor(s)
    public PrintWorkingDirectory(IShell shell) : base(shell) {}
    
    #endregion

    #region Methods
    public override void Execute(object? args = null)
    {
        string output = Directory.GetCurrentDirectory();

        if (IsStdOutRedirected)
        {
            StandardOutput = StreamReaderFromString(output);
            
            return;

        }
        
        Console.WriteLine(output);

    }

    #endregion

}