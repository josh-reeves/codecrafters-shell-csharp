using Interfaces;

namespace Shell.Core.Commands;

public class History : ShellCommand
{
    const int indent = 4;

    #region Constructor
    public History (IShell shell) : base(shell) {}

    #endregion

    #region Methods
    public override void Execute(object? args)
    {
        string output = string.Empty,
               str = string.Empty;

        int i,
            max = Shell.InputHistory.Count;

        IList<string> argList = args as IList<string> ?? [];

        if (int.TryParse(string.Join(' ', argList), out int temp) && temp < max)
        {
            max = temp;

        }

        for (i = 0; i < max - 1; i++)
        {
            str = $"{i + 1} {Shell.InputHistory[i]}\n";
            
            output += str.PadLeft(indent + str.Length);
            
        }
            
        str = $"{i + 1} {Shell.InputHistory[i]}";
        output += str.PadLeft(indent + str.Length);

        if (IsStdOutRedirected)
        {
            StandardOutput = StreamReaderFromString(output);
            
        }

        Console.WriteLine(output);
        
    }

    #endregion

    

}