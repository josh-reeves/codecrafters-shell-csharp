using Interfaces;

namespace Shell.Core.Input.Reader;

public class ShellReader : IShellReader, IDebuggable
{
    public ShellReader(string prompt = "", IDictionary<ConsoleKeyInfo, Func<string, ConsoleKeyInfo, string>>? keyMap = null)
    {
        Prompt = prompt;
        
        KeyMap = keyMap ?? new Dictionary<ConsoleKeyInfo, Func<string, ConsoleKeyInfo, string>>();
        
    }

    #region Events
    public event EventHandler<ConsoleKeyInfo>? InputReceived;

    #endregion

    #region Properties
    public bool Active { get; set; }

    public string Prompt { get; set; }

    public IDictionary<ConsoleKeyInfo, Func<string, ConsoleKeyInfo, string>> KeyMap { get; }

    public IDebugger? Debugger { get; set; }

    #endregion

    #region Methods
    public string Read(string prompt = "")
    {
        string input = string.Empty;

        if (string.IsNullOrEmpty(prompt))
        {
            prompt = Prompt;

        }

        Console.Write(prompt);

        Active = true;

        while (Active)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);


            BroadcastInput(keyInfo);

            Func<string, ConsoleKeyInfo, string>? func = RetrieveKeyMap(KeyMap, keyInfo);


            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                Console.WriteLine("test");
                Console.WriteLine(func == null);
                
            }

            if (func is not null)
            {
                Debugger?.WriteLine($"Executing mapped action: {func.Method.Name}", ["INPUT"]);

                input = func(input, keyInfo);

                continue;

            }

            if (!char.IsControl(keyInfo.KeyChar))
            {
                Debugger?.WriteLine($"Adding character to input string: {keyInfo.KeyChar}", ["INPUT"]);

                input += keyInfo.KeyChar;
                
                Console.Write(keyInfo.KeyChar);

            }

        }

        Debugger?.WriteLine($"Exiting input loop.", ["INPUT"]);

        Console.WriteLine();

        return input;

    }

    public void ClearLine(int startPos = 0)
    {
        int left = Console.GetCursorPosition().Left;
        int top = Console.GetCursorPosition().Top;

        for(int i = startPos; i <= left; i++)
        {
            Console.SetCursorPosition(i, top);

            Console.Write(' ');

        }

        Console.SetCursorPosition(startPos, top);
        
    }

    private Func<string, ConsoleKeyInfo, string>? RetrieveKeyMap(IDictionary<ConsoleKeyInfo, Func<string, ConsoleKeyInfo, string>> map, ConsoleKeyInfo keyInfo)
    {
        foreach (ConsoleKeyInfo compare in map.Keys)
        {
            Debugger?.WriteLine($"Comparing {keyInfo.Modifiers}{keyInfo.Key} to {compare.Modifiers}{compare.Key}", ["INPUT"]);

            if (MeetsKeyModifierMinimum(keyInfo, compare))
            {
                return map[compare];
                
            }
            
        }
        
        return null;

    }

    private bool MeetsKeyModifierMinimum(ConsoleKeyInfo keyInfo, ConsoleKeyInfo compare)
    {
        if (keyInfo.Key == compare.Key && (keyInfo.Modifiers & compare.Modifiers) == compare.Modifiers)
        {
            Debugger?.WriteLine("Key and minimum modifier requirements met.", ["INPUT"]);
            return true;
        }
        
        return false;
        
    }

    private void BroadcastInput(ConsoleKeyInfo keyInfo)
    {
        Debugger?.WriteLine($"Keypress received: {keyInfo.Modifiers}{keyInfo.Key}", ["INPUT"]);

        InputReceived?.Invoke(this, keyInfo);
        
    }

    #endregion

}