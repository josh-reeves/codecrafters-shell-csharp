namespace Interfaces;

public enum TokenType
{
    Word,
    RedirectStdOut,
    RedirectStdErr,
    AppendStdOut,
    AppendStdErr,
    Filename,
    Pipe,
    Expansion

}

public interface IShellToken : IToken
{
    #region Properties
    public bool IsQuoted { get; set; }
    
    public TokenType Type { get; }

    #endregion

}
