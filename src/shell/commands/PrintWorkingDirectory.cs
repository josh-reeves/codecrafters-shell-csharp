using Interfaces;

namespace Shell.Commands;

public class PrintWorkingDirectory : ShellCommand
{
    public PrintWorkingDirectory(IShell shell) : base(shell) {}
    
    public override void Execute(object? args = null)
    {
        StandardOutput += Directory.GetCurrentDirectory();

        if (IsStdOutRedirected)
        {
            return;

        }
        
        Console.WriteLine(StandardOutput);

    }

}