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
        bool reverse = false;
        int i,
            max = Shell.InputHistory.Count - 1;
        string output = string.Empty,
               str = string.Empty;

        IList<string> argList = args as IList<string> ?? [];

        if (int.TryParse(string.Join(' ', argList), out int temp))
        {
            max = temp < Shell.InputHistory.Count - 1 ? temp : max;
            reverse = true;

        }

        if (reverse)
        {
            for (i = max; i > 0; i--)
            {
                str = $"{i + 1} {Shell.InputHistory[i]}\n";
                
                output += str.PadLeft(indent + str.Length);
                
            }

        }
        else
        {
            for (i = 0; i < max; i++)
            {
                str = $"{i + 1} {Shell.InputHistory[i]}\n";
                
                output += str.PadLeft(indent + str.Length);
                
            }            

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