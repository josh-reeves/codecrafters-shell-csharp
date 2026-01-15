using Interfaces;

namespace Shell.Commands;

public class Echo : ShellCommand
{
    public Echo(IShell shell) : base(shell) {}

    public override void Execute(object? args)
    {                
        if (args is null)
        {
            return;

        }

        string[] argList = args as string[] ?? [];

        for (int i = 0; i < argList.Length; i++)
        {
            StandardOutput += argList[i];

            if (i == argList.Length - 1)
            {
                StandardOutput += "\n";

            }
            else
            {
                StandardOutput += Shell.CommandSeparator;
                
            }
            
        }

        if (Shell.IsStdOutRedirected)
        {
            return;

        }   

        Console.Write(StandardOutput);

    }
    
}