using Interfaces;
using Shell.Extensions.Parser.Tokens;

namespace Shell.Extensions.Parser;

public class ParserDefaultState : ParserState
{
    public ParserDefaultState() {}

    public ParserDefaultState(IParserStateController stateController) : base(stateController) {}

    public override void Enter()
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        controller.CurrentToken = new WordToken();

    }

    public override void Execute() 
    {
        if (Controller is not ParserStateController controller || controller.CurrentToken is null)
            return;

        if (controller.Parser.Separators.Contains(controller.RemainingText[0]))
        {
            controller.Transition(new ParserSeparatorState(controller));

            return;
            
        }

        controller.CurrentToken.Value += controller.RemainingText[0];

        controller.RemainingText = controller.RemainingText[1..];

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(new ParserEOFState(controller));
            
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
