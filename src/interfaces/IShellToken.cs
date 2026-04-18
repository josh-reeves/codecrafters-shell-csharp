namespace Interfaces;

public enum TokenType
{
    Word,
    RedirectStdOut,
    RedirectStdErr,
    AppendStdOut,
    AppendStdErr,
    Filename,
    Pipe

}

public interface IShellToken : IToken
{
    #region Properties
    public TokenType Type { get; }

    #endregion

}
