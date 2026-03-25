namespace Interfaces;

public interface IExpander
{
    #region Properties
    public IDictionary<string, Func<string, string>> ExpansionMap { get; }

    #endregion

    #region Method
    public Queue<IToken> Expand(Queue<IToken> tokens);

    #endregion
}
