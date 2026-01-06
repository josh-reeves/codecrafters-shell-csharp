using Interfaces;

namespace Shell.Commands;

public class PrintWorkingDirectory : ShellCommand
{
    public PrintWorkingDirectory(IShell shell) : base(shell) {}
    
    public override void Execute(object[]? args)
    {
        StandardOutput += Directory.GetCurrentDirectory();

        if (Shell.IsStdOutRedirected)
        {
            return;

        }
        
        Console.WriteLine(StandardOutput);

    }

}