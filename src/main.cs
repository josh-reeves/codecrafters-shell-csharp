using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Shell.Extensions.ShellInputHandler.Parser;

namespace Shell;

class Program
{
    static void Main()
    {
        Shell shell = new('$', "PATH", ' ', '~', new ShellInputHandler(new Lexer(), new Expander(), new Parser()));

        shell.Run();
        
    }

}
