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
            Console.Write(args[i]);

            if (i == args.Length - 1)
            {
                Console.Write("\r\n");
            }
            else
            {
                Console.Write(Shell.CommandSeparator);
                
            }
            
        }   

    }
    
}