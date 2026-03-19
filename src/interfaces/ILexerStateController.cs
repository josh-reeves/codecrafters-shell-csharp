namespace Interfaces;

public interface ILexerStateController : IStateController
{
#region Properties
    public int Position { get; set; }

    public string RemainingText { get; set; }

    public IState DefaultState { get; }

    public IToken? CurrentToken { get; set; }

    public Queue<IToken> TokenizedInput { get; set; }

    public IDictionary<string, IState> StateMap { get; }

    public IDictionary<IState, Func<IToken>> TokenMap { get; }

#endregion

#region Methods
    public void ConsumeInput(int numberOfCharacters = 1);

    public void AppendToken();

#endregion

}
