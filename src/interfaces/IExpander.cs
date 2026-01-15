namespace Interfaces;

public interface IExpander
{
    public IList<char> GroupDelimiters { get; }

    public Queue<IToken> Expand(Queue<IToken> tokens);

}
