using Shell.Extensions.ShellInputHandler.Lexer.Tokens;

namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public class LexerGroupDelimiterState : LexerState
{
    private char terminator;

    public LexerGroupDelimiterState(char terminatorChar)
    {
        terminator = terminatorChar;

    }

    public override void Enter()
    {
        if (Controller is not LexerStateController controller)
        {
            return;
 
        }

        controller.Lexer.CurrentToken ??= new WordToken()
        {
            Position = controller.Lexer.Position

        };

    }

    public override void Execute()
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        controller.ConsumeNext();

        MissingDelimiterCheck();

        if (controller.Lexer.RemainingText[0] == terminator)
        {
            controller.Transition(new LexerDefaultState());

        }

        controller.ConsumeNext();

        if (string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {            
            controller.Transition(new LexerEOFState());
        
        }

    }

    private void MissingDelimiterCheck()
    {
        if (Controller is not LexerStateController controller || !string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            return;

        }

        string prompt = "delim> ";

        Console.Write(prompt);
        controller.Lexer.RemainingText += Console.ReadLine();
        
    }

}
