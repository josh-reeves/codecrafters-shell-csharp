using Interfaces;

namespace Shell.Extensions.Parser.State;

public class ParserStateController : IParserStateController
{
    private IState defaultState,
                   currentState;

    public ParserStateController(IParser parser, IState initialState)
    {
        RemainingText = string.Empty;
        Parser = parser;

        ParsedTokens = [];

        currentState = defaultState = initialState;
        currentState.Controller = this;
        CurrentState.Enter();

    }

    public string RemainingText { get; set; }

    public IToken? CurrentToken { get; set; }

    public IParser Parser { get; private set;}

    public IState CurrentState
    {
        get => currentState;

        set
        {
            currentState = value;
            currentState.Controller = this;

        }
        
    }

    public IList<IToken> ParsedTokens { get; private set; }

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
