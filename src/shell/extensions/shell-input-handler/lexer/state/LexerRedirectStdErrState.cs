using System;
using Shell.Extensions.Lexer.Tokens;

namespace Shell.Extensions.Lexer.State;

public class LexerRedirectStdErrState : LexerState
{
    #region Fields
    private string sequence;

    #endregion

    #region Constructor(s)
    public LexerRedirectStdErrState(string operatorSequence)
    {
        sequence = operatorSequence;
        
    }

    #endregion

    #region Methods
    public override void Enter()
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        controller.Lexer.CurrentToken = new RedirectStdErrToken();

    }

    public override void Execute()
    {
        if (Controller is not LexerStateController controller || controller.Lexer.CurrentToken is null)
        {
            return;

        }

        if (controller.Lexer.CurrentToken.RawValue == sequence)
        {
            controller.Transition(new LexerDefaultState());

            return;

        }

        controller.Lexer.CurrentToken.RawValue += controller.Lexer.RemainingText[0];
        controller.Lexer.RemainingText = controller.Lexer.RemainingText[1..];
        controller.Lexer.Position++;

        if (string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            controller.Transition(new LexerEOFState());

            return;
            
        }

    }

    public override void Exit()
    {
        if (Controller is not LexerStateController controller || controller.Lexer.CurrentToken is null)
        {
            return;

        }

        if (string.IsNullOrWhiteSpace(controller.Lexer.CurrentToken.RawValue))
        {
            return;
            
        }

        controller.Lexer.TokenizedInput.Add(controller.Lexer.CurrentToken);

    }

    #endregion

}
