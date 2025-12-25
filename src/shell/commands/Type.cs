using Interfaces;

namespace Shell.Commands;

public class Type : ShellCommand
{
    private const string cmdNotFoundMsg = ": not found",
                            builtinMsg = " is a shell builtin";


    public Type(IShellEnvironment environment) : base(environment) {}

    public override void Execute(object[]? args)
    {
        if (args is null || args.Length <= 0)
        {
            return;

        }

        string arg = args[0] as string ?? string.Empty;

        if (Environment.Commands.Keys.Contains(arg))
        {
            Console.WriteLine(arg + builtinMsg);

            return;
            
        }

        foreach (string result in Environment.Search(arg, Environment.PathList))
        {
            if (Environment.IsExecutable([result]))
            {
                Console.WriteLine(args[0] + " is " + result);

                return;
                
            }
            
        }

        Console.WriteLine(args[0] + cmdNotFoundMsg);   

    }

}