namespace Interfaces;

public interface IExpander
{
    public IList<char> GroupDelimiters { get; }

    public IList<IToken> Expand(IList<IToken> tokens);

}
