using Interfaces;

namespace Shell.Extensions.Parser.State;

public class ParserSeparatorState : ParserState
{
    public ParserSeparatorState() {}

    public override void Execute()
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        if (!controller.Parser.Separators.Contains(controller.RemainingText[0]))
        {
            controller.Transition(new ParserDefaultState());

            return;

        }

        controller.RemainingText = controller.RemainingText[1..];
        
    }

}
