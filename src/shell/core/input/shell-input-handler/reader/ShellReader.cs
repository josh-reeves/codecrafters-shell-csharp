using Interfaces;

namespace Shell.Core.Input.Reader;

public class ShellReader : IShellReader, IDebuggable
{
    private string input;

    private Cursor cursor;

    public ShellReader(string prompt = "", IDictionary<ConsoleKeyInfo, Func<string, ConsoleKeyInfo, string>>? keyMap = null)
    {
        input = string.Empty;

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
        input = string.Empty;

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

    public void ClearLine()
    {
        if (input.Length <= 0 )
        {
            return;

        }

        cursor.MoveLeft(input.Length);

        for(int i = 1; i <= input.Length; i++)
        {
            Console.Write(' ');

        }

        cursor.MoveLeft(input.Length);

    }

    public void Insert(string input, int startPos = 0)
    {
        int top = Console.GetCursorPosition().Top;

        Console.SetCursorPosition(startPos, top);

        Console.Write(input);

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

    #region Structs
    private struct Cursor : ICursor
    {
        private const char Escape = '\u001B';
        private string escapePrefix => $"{Escape}[";

        public Cursor() {}

        public void MoveUp(int count = 1) => Console.Write($"{escapePrefix}{count}A");
        
        public void MoveDown(int count = 1) => Console.Write($"{escapePrefix}{count}B");

        public void MoveLeft(int count = 1) => Console.Write($"{escapePrefix}{count}D");
        
        public void MoveRight(int count = 1) => Console.Write($"{escapePrefix}{count}C");
        
        public void SetColumn(int count) => Console.Write($"{escapePrefix}{count}G");
                
        public (int row, int col) GetPosition()
        {
            int row = -1,
                col = -1;

            Console.Write($"{escapePrefix}6n");
           
            ConsoleKeyInfo keyInfo;
            
            string output = string.Empty;

            while (Console.KeyAvailable && (keyInfo = Console.ReadKey(true)).KeyChar != 'R')
            {
                output += keyInfo.KeyChar;
                
            }

            output = output.Substring(2, output.Length - 2);
            string[] coords = output.Split(';');
        
            int.TryParse(coords[0], out row); 
            int.TryParse(coords[1], out col);
            
            return (row, col);
        
        }

        public void SetPosition(int row, int col) 
            => Console.WriteLine($"{escapePrefix}{row};{col}H");

    }

    #endregion

}