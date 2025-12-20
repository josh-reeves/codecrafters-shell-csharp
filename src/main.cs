class Program
{
    private const string invalidCommandMsg = ": command not found";

    static void Main()
    {
        Console.Write("$ ");

        Console.WriteLine(Console.ReadLine() + invalidCommandMsg);

    }

}
