class Program
{
    private const string invalidCmdMsg = ": command not found";

    static void Main()
    {
        bool replActive = true;
        string input;

        Dictionary<string, string> commands =
        [
            
            
        ];

        commands.Add("exit", "exit");

        while (replActive)
        {
            Console.Write("$ ");

            input = Console.ReadLine() ?? string.Empty;

            if (input == commands["exit"])
            {
                replActive = false;

                break;

            }

            Console.WriteLine(input + invalidCmdMsg);

        }

    }

}
