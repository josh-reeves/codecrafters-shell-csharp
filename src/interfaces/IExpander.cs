namespace Interfaces;

public interface IExpander
{
    #region Properties
    public IDictionary<string, Func<IToken, IToken>> ExpansionMap { get; }

    public IList<Func<string, string>> ExpansionMethods { get; }

    #endregion

    #region Method
    public Queue<IToken> Expand(Queue<IToken> tokens);

    #endregion
}
