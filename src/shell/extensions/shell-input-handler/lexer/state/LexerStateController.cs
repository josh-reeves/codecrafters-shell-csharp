using Interfaces;

namespace Shell.Extensions.Lexer.State;

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

}
