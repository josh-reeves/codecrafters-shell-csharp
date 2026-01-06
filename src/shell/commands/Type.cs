using Interfaces;

namespace Shell.Commands;

public class Type : ShellCommand
{
    private const string cmdNotFoundMsg = ": not found",
                            builtinMsg = " is a shell builtin";


    public Type(IShell shell) : base(shell) {}

    public override void Execute(object[]? arguments)
    {
        if (arguments is null || arguments.Length <= 0)
        {
            return;

        }

        string[] args = arguments as string[] ?? [];        
        List<string> results = [];

        foreach (string arg in arguments)
        {
            if (Shell.Commands.Keys.Contains(arg))
            {
                StandardOutput += arg + builtinMsg + "\n";

                continue;
                
            }

            results.AddRange(Shell.Search(arg, Shell.PathList));

        }

        foreach (string result in results)
        {
            if (Shell.IsExecutable([result]))
            {
                StandardOutput += result + " is " + result + "\n";

                continue;
                
            }

            StandardOutput += result + cmdNotFoundMsg + "\n";
            
        }

        if (Shell.IsStdOutRedirected)
        {
            return;

        }

        Console.Write(StandardOutput);

    }

}