using Interfaces;

namespace Shell.Core.Input.ShellInputHandler.Lexer.State;

public class LexerStateController : ILexerStateController, IDebuggable
{
    private IState currentState;

    #region Constructor(s)
    public LexerStateController(IState defaultState, IDictionary<IState, Func<IToken>>? tokenMap = null)
    {
        Position = 0;
        RemainingText = string.Empty;   
        TokenizedInput =[];

        CurrentState = currentState = DefaultState = defaultState;

        StateMap = new Dictionary<string, IState>();
        TokenMap = tokenMap ?? new Dictionary<IState, Func<IToken>>();

        CurrentState.Enter();

    }

    #endregion

    #region Properties
    public int Position { get; set; }

    public string RemainingText { get; set; }

    public IToken? CurrentToken { get; set; }

    public IState DefaultState { get; private set; }

    public IDebugger? Debugger { get; set; }

    public IState CurrentState
    {
        get => currentState;

        set
        {
            currentState = value;
            currentState.Controller = this;
#if DEBUG
            Debugger?.WriteLine($"New lexer state set: {CurrentState.GetType().Name}");
            Debugger?.WriteLine($"Remaining: {RemainingText}");
#endif
        }
        
    }

    public Queue<IToken> TokenizedInput { get; set; }

    public IDictionary<string, IState> StateMap { get; }

    public IDictionary<IState, Func<IToken>> TokenMap { get; }

    #endregion
    
    #region Methods
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
            CurrentState = DefaultState;

        }

    }

    public void ConsumeInput(int numberOfCharacters = 1)
    {
        numberOfCharacters = numberOfCharacters > RemainingText.Length ? RemainingText.Length : numberOfCharacters;

        if (CurrentToken is not null)
        {
            CurrentToken.RawValue += RemainingText[..numberOfCharacters];

        }

        RemainingText = RemainingText[numberOfCharacters..];
        Position += numberOfCharacters;

    }

    public void AppendToken()
    {
        IToken? token = CurrentToken;
        CurrentToken = null;

        if (string.IsNullOrWhiteSpace(token?.RawValue))
        {
            return;

        }

        TokenizedInput.Enqueue(token);

    }

    #endregion
    
}