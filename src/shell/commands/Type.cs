using Interfaces;

namespace Shell.Commands;

public class Type : ShellCommand
{
    #region Fields
    private const string cmdNotFoundMsg = ": not found",
                            builtinMsg = " is a shell builtin";

    #endregion

    #region Constructor(s)
    public Type(IShell shell) : base(shell) {}

    #endregion

    #region Methods
    public override void Execute(object? args)
    {        
        if (args is null)
        {
            return;

        }
                        
        foreach (string arg in args as string[] ?? [])
        {
            string result = arg + cmdNotFoundMsg + '\n';

            foreach(string file in Shell.Search(arg, Shell.PathList))       
            {
                result = Shell.IsExecutable([file]) ? arg + " is " + file + '\n' : result;

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

    #endregion

}