namespace Interfaces;

public enum TokenType
{
    Word,
    RedirectStdOut,
    RedirectStdErr,
    AppendStdOut,
    AppendStdErr,
    Filename

}

public interface IShellToken : IToken
{
    public TokenType Type { get; }

}
