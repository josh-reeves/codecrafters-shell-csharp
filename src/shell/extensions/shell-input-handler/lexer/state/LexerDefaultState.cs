using Shell.Extensions.ShellInputHandler.Lexer.Tokens;



namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public class LexerDefaultState : LexerState
{
    private int maxOpLength;

    public LexerDefaultState() {}

    public override void Enter()
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        foreach (string op in controller.Lexer.Operators.Keys)
        {
            maxOpLength = op.Length > maxOpLength ? op.Length : maxOpLength;
            
        }

        controller.Lexer.CurrentToken = new WordToken();

    }

    public override void Execute() 
    {
        if (Controller is not LexerStateController controller || controller.Lexer.CurrentToken is null)
        {
            return;

        }

        if (controller.Lexer.Separators.Contains(controller.Lexer.RemainingText[0]))
        {
            controller.Transition(new ParserSeparatorState());

            return;
            
        }

        for (int i = maxOpLength; i > 0; i--)
        {
            string seq = controller.Lexer.RemainingText.Length >= i ? controller.Lexer.RemainingText[0..i] : string.Empty;

            if (controller.Lexer.Operators.ContainsKey(seq))
            {
                controller.Transition(controller.Lexer.Operators[seq]);
                return;

            }

        }

        if (controller.Lexer.GroupDelimiters.Keys.Contains(controller.Lexer.RemainingText[0]))
        {
            controller.Transition(controller.Lexer.GroupDelimiters[controller.Lexer.RemainingText[0]]);

            return;

        }

        controller.Lexer.CurrentToken.RawValue += controller.Lexer.RemainingText[0];
        controller.Lexer.RemainingText = controller.Lexer.RemainingText[1..];
        controller.Lexer.Position++;

        if (string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            controller.Transition(new LexerEOFState());
            
        }
        
    }

    public override void Exit()
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        if (string.IsNullOrWhiteSpace(controller.Lexer.CurrentToken?.RawValue))
        {
            return;
            
        }

        controller.Lexer.TokenizedInput.Add(controller.Lexer.CurrentToken);

    }

}
