using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;

namespace Shell;

class Program
{
    static void Main()
    {
        Shell shell = new("PATH", ' ', '~', new ShellInputHandler(new Lexer(), new Expander()));

        shell.Run();
        
    }

}
