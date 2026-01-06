using Interfaces;

namespace Shell.Commands;

public class Type : ShellCommand
{
    private const string cmdNotFoundMsg = ": not found",
                            builtinMsg = " is a shell builtin";


    public Type(IShell shell) : base(shell) {}

    public override void Execute(object[]? args)
    {
        if (args is null || args.Length <= 0)
        {
            return;

        }
        
        Queue<string> arguments = new Queue<string>(args as string[] ?? []);
        
        while (arguments.Count > 0)
        {
            if (Shell.Commands.ContainsKey(arguments.Peek()))
            {
                StandardOutput += arguments.Dequeue() + builtinMsg + "\n";

                continue;
                
            }

            foreach (string result in Shell.Search(arguments.Peek(), Shell.PathList))
            {
                if (Shell.IsExecutable([result]))
                {
                    StandardOutput += arguments.Dequeue() + " is " + result + "\n";
                    
                }
                
            }

            if (arguments.Count > 0 && !Shell.Commands.ContainsKey(arguments.Peek()))
            {
                StandardOutput += arguments.Dequeue() + cmdNotFoundMsg + "\n";
   
            }

        }

        if (Shell.IsStdOutRedirected)
        {
            return;

        }

        Console.Write(StandardOutput);

    }

}