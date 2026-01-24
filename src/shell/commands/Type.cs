using Interfaces;

namespace Shell.Commands;

public class Type : ShellCommand
{
    private const string cmdNotFoundMsg = ": not found",
                            builtinMsg = " is a shell builtin";


    public Type(IShell shell) : base(shell) {}

    public override void Execute(object? args)
    {        
        if (args is null)
        {
            return;

        }
        
        string[] argList = args as string[] ?? [];
                
        foreach (string arg in argList ?? [])
        {
            string result = arg + cmdNotFoundMsg + '\n';

            foreach(string file in Shell.Search(arg, Shell.PathList))       
            {
                if (Shell.IsExecutable([file]))
                {
                    result = arg + " is " + file + '\n';

                    continue;

                }

            }

            if (Shell.Builtins.ContainsKey(arg))
            {
                result = arg + builtinMsg + '\n';
                
            }

            StandardOutput += result;

        }

        if (IsStdOutRedirected)
        {
            return;

        }

        Console.Write(StandardOutput);

    }

}