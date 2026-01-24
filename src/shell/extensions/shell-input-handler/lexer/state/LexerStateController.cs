using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public class LexerStateController : ILexerStateController
{
    private IState defaultState,
                   currentState;

    public LexerStateController(ILexer lexer, IState initialState)
    {
        Lexer = lexer;

        currentState = defaultState = initialState;
        currentState.Controller = this;
        CurrentState.Enter();

    }

    public ILexer Lexer { get; set;}

    public IState CurrentState
    {
        get => currentState;

        set
        {
            currentState = value;
            currentState.Controller = this;

        }
        
    }

    public void Transition(IState state)
    {

        try
        {
            CurrentState.Exit();

            CurrentState = state;

            CurrentState.Enter();

        }
        catch
        {
            CurrentState = defaultState;

        }

    }

    public void ConsumeNext(int numberOfCharacters = 1)
    {
        if (Lexer.CurrentToken is null)
        {
            return;

        }

        Lexer.CurrentToken.RawValue += Lexer.RemainingText[..numberOfCharacters];
        Lexer.RemainingText = Lexer.RemainingText[numberOfCharacters..];
        Lexer.Position += numberOfCharacters;

    }

    public void AppendToken()
    {
        if (string.IsNullOrWhiteSpace(Lexer.CurrentToken?.RawValue))
        {
            return;

        }

        IToken token = Lexer.CurrentToken;

        Lexer.TokenizedInput.Enqueue(token);

        Lexer.CurrentToken = null;

    }

}
