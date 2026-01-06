using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer.Tokens;


public abstract class ShellToken : IToken
{
    public ShellToken()
    {
        RawValue = string.Empty;
        ExpandedValue = string.Empty;

    }

    public int Position { get; set; }

    public string RawValue { get; set; }

    public string ExpandedValue { get; set; }

}
