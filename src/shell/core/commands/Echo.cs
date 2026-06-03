using Interfaces;

namespace Shell.Core.Commands;

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

        string output = string.Empty;

        for (int i = 0; i < argList.Count; i++)
        {
            output += argList[i];

            if (i < argList.Count - 1)
            {
                output += Shell.CommandSeparator;

            }
            
        }

        if (IsStdOutRedirected)
        {
            StandardOutput = StreamReaderFromString(output);

            return;

        }

        Console.WriteLine(output);

    }
    
    #endregion

}