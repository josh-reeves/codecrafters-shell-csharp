using Interfaces;

namespace Shell.Core.Commands;

public class History : ShellCommand
{
    #region Constructor
    public History (IShell shell) : base(shell) {}

    #endregion

    #region Methods
    public override void Execute(object? args)
    {
        string output = string.Empty;

        int i;
        
        for (i = 0; i < Shell.InputHistory.Count - 1; i++)
        {
            output += $"\t{i + 1} {Shell.InputHistory[i]}\n";
            
        }

        output += $"\t{i + 1} {Shell.InputHistory[i]}";

        if (IsStdOutRedirected)
        {
            StandardOutput = StreamReaderFromString(output);
            
        }

        Console.WriteLine(output);
        
    }

    #endregion

    

}