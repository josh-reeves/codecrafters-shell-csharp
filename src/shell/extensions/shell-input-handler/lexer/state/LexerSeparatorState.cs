namespace Shell.Extensions.Lexer.State;

public class ParserSeparatorState : LexerState
{
    public ParserSeparatorState() {}

    public override void Execute()
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        if (!controller.Lexer.Separators.Contains(controller.Lexer.RemainingText[0]))
        {
            controller.Transition(new LexerDefaultState());

            return;

        }

        controller.Lexer.RemainingText = controller.Lexer.RemainingText[1..];
        controller.Lexer.Position++;
        
    }

}
