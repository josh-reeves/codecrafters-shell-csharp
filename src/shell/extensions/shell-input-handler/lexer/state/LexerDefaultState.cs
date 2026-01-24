using Interfaces;
using Shell.Extensions.ShellInputHandler.Lexer.Tokens;



namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public class LexerDefaultState : LexerState
{
    public LexerDefaultState() {}

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


        if (controller.Lexer.Separators.Contains(controller.Lexer.RemainingText[0]))
        {
            controller.Transition(new ParserSeparatorState());

            return;
            
        }

        for (int i = GetMaxOperatorLength(controller); i > 0; i--)
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

        controller.ConsumeNext();

        if (string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            controller.Transition(new LexerEOFState());
            
        }
        
    }

    private int GetMaxOperatorLength(ILexerStateController controller)
    {
        int maxLength = 0;

        foreach (string op in controller.Lexer.Operators.Keys)
        {
            maxLength = op.Length > maxLength ? op.Length : maxLength;
            
        }

        return maxLength;
        
    }

}
