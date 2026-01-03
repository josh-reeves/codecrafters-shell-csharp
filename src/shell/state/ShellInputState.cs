using Interfaces;
using Shell.Extensions.Parser.State;
using Shell.Extensions.Parser.Tokens;

namespace Shell.State;

public class ShellInputState : ShellState
{
    public ShellInputState() {}

    public override void Enter()
    {
        if (Controller is not ShellStateController controller)
        {
            return;

        }

        Console.SetOut(
            new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
                
            });

        controller.Shell.Command = string.Empty;
        controller.Shell.Filename = string.Empty;
        controller.Shell.Args = []; 
        controller.Shell.OutputWriter?.Close();
        controller.Shell.OutputWriter = null;
        controller.Shell.ShellEnvironment.IsOutputRedirected = false;

    }

    public override void Execute()
    {
        if (Controller is not ShellStateController controller)
        {
            return;

        }

        IShell shell = controller.Shell;

        Console.Write("$ ");

        string input = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
            
        }

        shell.Parser.TokenizedInput = shell.Parser.Tokenize(input, new ParserStateController(shell.Parser, new ParserDefaultState()));

        shell.Command = shell.Parser.TokenizedInput[0].Value;
        
        for(int i = 1; i < shell.Parser.TokenizedInput.Count; i++)
        {
            if (shell.Parser.TokenizedInput[i] is RedirectStdOutToken)
            {
                shell.Filename = shell.Parser.TokenizedInput[i + 1].Value;

                controller.Transition(new RedirectStdOutState());

                return;

            }

            shell.Args.Add(shell.Parser.TokenizedInput[i].Value);

        }

        controller.Transition(new ShellExecutionState());
        
    }

}
