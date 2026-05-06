using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer;

public class ShellToken : IShellToken
{
    #region Constructor(s)
    public ShellToken(TokenType type)
    {
        IsQuoted = false;
        RawValue = string.Empty;
        ExpandedValue = string.Empty;
        Type = type;

    }

    #endregion

    #region Properties
    public bool IsQuoted { get; set; }

    public int Position { get; set; }

    public string RawValue { get; set; }

    public string ExpandedValue { get; set; }

    public TokenType Type { get; }

    #endregion

}
