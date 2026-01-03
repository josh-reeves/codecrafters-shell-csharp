namespace Shell;

class Program
{
    static void Main()
    {
        Shell shell = new("PATH", ' ', '~');

        shell.Run();
        
    }

}
