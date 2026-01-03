using Interfaces;
using Shell.Commands;
using Shell.Extensions.Parser;
using Shell.Extensions.Parser.State;
using Shell.State;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
    #region Fields
    private ShellStateController controller;

    #endregion

    #region Constructor(s)
    public Shell()
    {        
        Command = string.Empty;
        Filename = string.Empty;
        Args = [];
        Parser = new Parser();
        ShellEnvironment = new ShellEnvironment(this, ' ', '~', "PATH");

        controller = new(this, new ShellInputState());

        Parser.Separators.Add(ShellEnvironment.CommandSeparator);
        Parser.GroupDelimiters.Add('\'', new ParserGroupDelimiterState('\''));
        Parser.GroupDelimiters.Add('"', new ParserGroupDelimiterState('"'));
        Parser.Operators.Add(">", new ParserRedirectStdOutState(">"));
        Parser.Operators.Add("1>", new ParserRedirectStdOutState("1>"));

        ShellEnvironment.Commands.Add("exit", new Exit(ShellEnvironment));
        ShellEnvironment.Commands.Add("echo", new Echo(ShellEnvironment));
        ShellEnvironment.Commands.Add("type", new Type(ShellEnvironment));
        ShellEnvironment.Commands.Add("pwd", new PrintWorkingDirectory(ShellEnvironment));
        ShellEnvironment.Commands.Add("cd", new ChangeDirectory(ShellEnvironment));

    }

    #endregion

    #region Properties
    public string Command { get; set; }

    public string Filename { get; set; }

    public IShellEnvironment ShellEnvironment { get; private set; }

    public IParser Parser { get; private set;}

    public StreamWriter? OutputWriter { get; set; }

    public IList<string> Args { get; set; }

    #endregion

    #region Methods
    public void Run()
    {
        ShellEnvironment.ShellIsActive = true;

        while (ShellEnvironment.ShellIsActive)
        {
            try
            {
               controller.CurrentState.Execute();
             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }

        }

    }

    #endregion
        
}

    