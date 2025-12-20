class Program
{
    private const char sep = ' ';
    private const string invalidCmdMsg = ": command not found";

    static void Main()
    {
        bool replActive = true;
        string input = string.Empty;

        Dictionary<string, Action> commands = new()
        {
            {"exit", () => replActive = false},
            {"echo", () => echo() }

        };
        
        // Main REPL loop.
        while (replActive)
        {
            Console.Write("$ ");

            input = Console.ReadLine() ?? string.Empty;

            string command = input.Split(sep)[0];

            if (!commands.Keys.Contains(command))
            {
                Console.WriteLine(command + invalidCmdMsg);
                
            }
            else
            {
                commands[command]?.Invoke();

            }

        }

#region Local Functions
        void echo()
        {
            string[] inputArray = input.Split(sep);

            for (int i = 1; i < inputArray.Length; i++)
            {
                Console.Write(inputArray[i]);

                if (i == inputArray.Length - 1)
                {
                    Console.Write("\r\n");
                }
                else
                {
                    Console.Write(sep);
                    
                }
                
            }

#endregion
            
        }


    }


}
