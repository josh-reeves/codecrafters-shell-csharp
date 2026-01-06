using Interfaces;

namespace Shell.Commands;

public class Echo : ShellCommand
{
    public Echo(IShell shell) : base(shell) {}

    public override void Execute(object[]? args)
    {                
        if (args is null)
        {
            return;

        }

        for (int i = 0; i < args.Length; i++)
        {
            StandardOutput += (args[i]);

            if (i > args.Length - 1)
            {
                StandardOutput += Shell.CommandSeparator;

            }
 
            
        }

        if (Shell.IsStdOutRedirected)
        {
            return;

        }   

        Console.WriteLine(StandardOutput);

    }
    
}