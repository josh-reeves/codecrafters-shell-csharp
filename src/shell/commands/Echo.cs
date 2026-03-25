using Interfaces;

namespace Shell.Commands;

public class Echo : ShellCommand
{
    #region Constructor(s)
    public Echo(IShell shell) : base(shell) {}

    #endregion

    #region Methods
    public override void Execute(object? args)
    {                        
        if (args is null)
        {
            return;

        }

        IList<string> argList = args as IList<string> ?? [];

        for (int i = 0; i < argList.Count; i++)
        {
            StandardOutput += argList[i];

            if (i == argList.Count - 1)
            {
                StandardOutput += "\n";

            }
            else
            {
                StandardOutput += Shell.CommandSeparator;
                
            }
            
        }

        if (IsStdOutRedirected)
        {
            return;

        }

        Console.Write(StandardOutput);

    }
    
    #endregion

}