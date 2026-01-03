using Interfaces;
using Shell.Extensions.Parser.Tokens;

namespace Shell.Extensions.Parser.State;

public class ParserDefaultState : ParserState
{
    private int maxOpLength;

    public ParserDefaultState() {}

    public override void Enter()
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        foreach (string op in controller.Parser.Operators.Keys)
        {
            maxOpLength = op.Length > maxOpLength ? op.Length : maxOpLength;
            
        }

        controller.CurrentToken = new WordToken();

    }

    public override void Execute() 
    {
        if (Controller is not ParserStateController controller || controller.CurrentToken is null)
        {
            return;

        }

        if (controller.Parser.Separators.Contains(controller.RemainingText[0]))
        {
            controller.Transition(new ParserSeparatorState());

            return;
            
        }

        for (int i = maxOpLength; i > 0; i--)
        {
            string seq = controller.RemainingText.Length >= i ? controller.RemainingText[0..i] : string.Empty;

            if (controller.Parser.Operators.ContainsKey(seq))
            {
                controller.Transition(controller.Parser.Operators[seq]);

                return;

            }

        }

        if (controller.Parser.GroupDelimiters.Keys.Contains(controller.RemainingText[0]))
        {
            controller.Transition(controller.Parser.GroupDelimiters[controller.RemainingText[0]]);

            return;

        }

        controller.CurrentToken.Value += controller.RemainingText[0];

        controller.RemainingText = controller.RemainingText[1..];

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(new ParserEOFState());
            
        }
        
    }

    public override void Exit()
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        if (string.IsNullOrWhiteSpace(controller.CurrentToken?.Value))
        {
            return;
            
        }

        controller.ParsedTokens.Add(controller.CurrentToken);

    }

}
