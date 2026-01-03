using Shell.Extensions.Parser.Tokens;

namespace Shell.Extensions.Parser.State;

public class ParserRedirectStdOutState : ParserState
{
    #region Fields
    private string sequence;

    #endregion

    #region Constructor(s)
    public ParserRedirectStdOutState(string charSequence)
    {
        sequence = charSequence;
        
    }

    #endregion

    #region Methods
    public override void Enter()
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        controller.CurrentToken = new RedirectStdOutToken();

    }

    public override void Execute()
    {
        if (Controller is not ParserStateController controller || controller.CurrentToken is null)
        {
            return;

        }

        if (controller.CurrentToken.Value == sequence)
        {
            controller.Transition(new ParserDefaultState());

            return;

        }

        controller.CurrentToken.Value += controller.RemainingText[0];
        controller.RemainingText = controller.RemainingText[1..];

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(new ParserEOFState());

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

    #endregion

}
