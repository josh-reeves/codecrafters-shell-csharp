namespace Interfaces;

public interface IShellInputHandler
{
    public ILexer Lexer { get; }

    public IExpander Expander { get; }

    public IList<IToken> ReadInput(string input);

}
