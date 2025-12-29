using Interfaces;

namespace Shell.Extensions.Parser;

public class ParserStateController : IParserStateController
{
    private IState defaultState;

    public ParserStateController(IParser parser, IState initialState)
    {
        RemainingText = string.Empty;
        Parser = parser;

        ParsedTokens = [];

        initialState.Controller ??= this;
        CurrentState = defaultState = initialState;
        CurrentState.Enter();

    }

    public string RemainingText { get; set; }

    public IToken? CurrentToken { get; set; }

    public IParser Parser { get; private set;}

    public IState CurrentState { get; private set; }

    public IList<IToken> ParsedTokens { get; private set; }

    public void Transition(IState state)
    {
        CurrentState.Exit();

        CurrentState = state;

        CurrentState.Enter();

    }

}
