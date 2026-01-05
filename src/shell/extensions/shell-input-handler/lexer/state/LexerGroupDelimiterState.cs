using Shell.Extensions.Lexer.Tokens;

namespace Shell.Extensions.Lexer.State;

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

        controller.Lexer.CurrentToken = new WordToken();

    }

    public override void Execute()
    {
        if (Controller is not LexerStateController controller || controller.Lexer.CurrentToken is null)
        {
            return;

        }

        controller.Lexer.CurrentToken.RawValue += controller.Lexer.RemainingText[0];

        controller.Lexer.RemainingText = controller.Lexer.RemainingText[1..];
        controller.Lexer.Position++;

        if (string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            controller.Transition(new LexerEOFState());
            
            return;
        }

        if (controller.Lexer.RemainingText[0] == terminator)
        {
            controller.Lexer.CurrentToken.RawValue += controller.Lexer.RemainingText[0];
            controller.Lexer.RemainingText = controller.Lexer.RemainingText[1..];
            controller.Lexer.Position++;

            controller.Transition(new LexerDefaultState());

            return;

        }

    }

    public override void Exit()
    {
        if (Controller is not LexerStateController controller || controller.Lexer.CurrentToken is null)
        {
            return;

        }

        if (string.IsNullOrWhiteSpace(controller.Lexer.CurrentToken.RawValue))
        {
            return;
            
        }

        controller.Lexer.TokenizedInput.Add(controller.Lexer.CurrentToken);

    }

}
