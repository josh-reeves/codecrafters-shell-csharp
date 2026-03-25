using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer;

public class ShellToken : IShellToken
{
    public ShellToken(TokenType type)
    {
        RawValue = string.Empty;
        ExpandedValue = string.Empty;
        Type = type;

    }

    public int Position { get; set; }

    public string RawValue { get; set; }

    public string ExpandedValue { get; set; }

    public TokenType Type { get; }

}
