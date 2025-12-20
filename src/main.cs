class Program
{
    private const string invalidCommandMsg = ": command not found";

    static void Main()
    {
        bool shellLoopActive = true;

        while (shellLoopActive)
        {
            Console.Write("$ ");

            Console.WriteLine(Console.ReadLine() + invalidCommandMsg);

        }

    }

}
