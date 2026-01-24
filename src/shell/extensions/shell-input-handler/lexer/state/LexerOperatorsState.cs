using Shell.Extensions.ShellInputHandler.Lexer.Tokens;

namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public class LexerOperatorState : LexerState
{
    #region Fields
    private string sequence;

    #endregion

    #region Constructor(s)
    public LexerOperatorState(string operatorsSequence)
    {
        sequence = operatorsSequence;
        
    }

    #endregion

    #region Methods
    public override void Enter()
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        controller.AppendToken();
 
        controller.Lexer.CurrentToken = controller.Lexer.Operators[sequence].Invoke();
        controller.Lexer.CurrentToken.Position = controller.Lexer.Position;


    }

    public override void Execute()
    {
        if (Controller is not LexerStateController controller || controller.Lexer.CurrentToken is null)
        {
            return;

        }

        controller.ConsumeNext();

        if (string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            controller.Transition(new LexerDefaultState());

            return;
            
        }

        if (controller.Lexer.CurrentToken.RawValue == sequence)
        {
            controller.Transition(new LexerDefaultState());

        }

    }

    #endregion

}
