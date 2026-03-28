namespace Interfaces;

public interface IExpander
{
    #region Properties
    public IDictionary<char, Func<string, (string original, string expansion)>> ExpansionMap { get; }

    #endregion

    #region Method
    public Queue<IToken> Expand(Queue<IToken> tokens);

    #endregion
}
