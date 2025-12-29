using Interfaces;

namespace Shell.Extensions.Parser;

public class ParserSeparatorState : ParserState
{
    public ParserSeparatorState() {}

    public ParserSeparatorState(IParserStateController stateController) : base(stateController) {}

    public override void Execute()
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        if (!controller.Parser.Separators.Contains(controller.RemainingText[0]))
        {
            controller.Transition(new ParserDefaultState(controller));

            return;

        }

        controller.RemainingText = controller.RemainingText[1..];

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(new ParserEOFState(controller));
            
        }
        
    }

}
