namespace Interfaces;

public interface IShellInputHandler
{
    public ILexer Lexer { get; }

    public IExpander Expander { get; }

    public ITree ReadInput(string input);

}
