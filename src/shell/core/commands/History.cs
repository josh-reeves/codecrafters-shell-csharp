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

        foreach (string str in Shell.InputHistory)
        {
            output += str + '\n';
            
        }

        if (IsStdOutRedirected)
        {
            StandardOutput = StreamReaderFromString(output);
            
        }

        Console.WriteLine(output);
        
    }

    #endregion

    

}