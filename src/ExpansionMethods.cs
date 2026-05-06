using Interfaces;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;

namespace Shell;

static class ExpansionMethods
{
    #region Constructor(s)
    static ExpansionMethods()
    {
        Expander = new Expander();
        
    }

    #endregion

    #region Properties
    public static IExpander Expander { get; set; }

    #endregion

    #region Methods
    public static IToken ExpandEscape(IToken token)
    {
        ShellToken expansion = new(TokenType.Expansion)
        {
            RawValue = ShellChars.Escape.Sequence,
            ExpandedValue = string.Empty
            
        };

        if (token is IShellToken shellToken && shellToken.IsQuoted)
        {
            expansion.ExpandedValue = ShellChars.Escape.Sequence;

        }

        return expansion;

    }

    public static IToken ExpandNewLine(IToken token)
    {
        ShellToken expansion = new(TokenType.Expansion)
        {
            RawValue = ShellChars.NewLine.Sequence,
            ExpandedValue = "\n"
            
        };

        return expansion;

    }

    public static IToken ExpandSingleQuote(IToken token)
    {
        string input = token.RawValue;
        char quoteChar = input[0];        
        int end = input.IndexOf(quoteChar, 1) >= 1 ? input.IndexOf(quoteChar, 1) : input.Length;

        ShellToken expansion = new(TokenType.Expansion)
        {
            RawValue = input[0..(end < input.Length ? end + 1 : end)],
            ExpandedValue = input[1..end]
            
        };
        
        return expansion;

    }

    public static IToken ExpandDoubleQuote(IToken token)
    {   
        IShellToken expansion = (IShellToken)ExpandSingleQuote(token);
        IShellToken temp = new ShellToken(TokenType.Expansion)
        {
            RawValue = expansion.ExpandedValue,
            IsQuoted = true
            
        };

        Expander.Expand(new Queue<IToken>([temp]));

        expansion.ExpandedValue = temp.ExpandedValue;

        return expansion;

    } 

    public static IToken ExpandHome(IToken token)
    {
        ShellToken expansion = new(TokenType.Expansion)
        {
            RawValue = ShellChars.Home.Sequence,
            ExpandedValue = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            
        };

        return expansion;

    }

    #endregion

}
