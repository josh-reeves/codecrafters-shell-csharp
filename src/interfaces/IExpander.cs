namespace Interfaces;

public interface IExpander
{
    #region Properties
    public IList<char> GroupDelimiters { get; }

    public IList<char> EscapeCharacters { get; }

    #endregion

    #region Method
    public Queue<IToken> Expand(Queue<IToken> tokens);

    #endregion
}
