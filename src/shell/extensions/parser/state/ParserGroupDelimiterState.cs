using Interfaces;
using Shell.Extensions.Parser.Tokens;

namespace Shell.Extensions.Parser.State;

public class ParserGroupDelimiterState : ParserState
{
    private char terminator;

    public ParserGroupDelimiterState(char terminatorChar)
    {
        terminator = terminatorChar;

    }

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
        {
            return;

        }

        controller.CurrentToken.Value += controller.RemainingText[0];

        controller.RemainingText = controller.RemainingText[1..];

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(new ParserEOFState());
            
            return;
        }

        if (controller.RemainingText[0] == terminator)
        {
            controller.CurrentToken.Value += controller.RemainingText[0];
            controller.RemainingText = controller.RemainingText[1..];

            controller.Transition(new ParserDefaultState());

            return;

        }

    }

    public override void Exit()
    {
        if (Controller is not ParserStateController controller || controller.CurrentToken is null)
        {
            return;

        }

        if (string.IsNullOrWhiteSpace(controller.CurrentToken.Value))
        {
            return;
            
        }

        controller.ParsedTokens.Add(controller.CurrentToken);

    }

}
