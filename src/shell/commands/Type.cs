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

        string arg = args[0] as string ?? string.Empty;

        if (Shell.Commands.Keys.Contains(arg))
        {
            Console.WriteLine(arg + builtinMsg);

            return;
            
        }

        foreach (string result in Shell.Search(arg, Shell.PathList))
        {
            if (Shell.IsExecutable([result]))
            {
                Console.WriteLine(args[0] + " is " + result);

                return;
                
            }
            
        }

        Console.WriteLine(args[0] + cmdNotFoundMsg);   

    }

}