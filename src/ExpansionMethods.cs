using Interfaces;
using Shell.Core.Input.ShellInputHandler.Expander;
using Shell.Core.Input.ShellInputHandler.Lexer;

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
            ExpandedValue = string.Empty,
            IsQuoted = true
            
        };

        if (!((IShellToken)token).IsQuoted)
        {
            return expansion;
        
        }

        IList<string> chars = [ShellChars.Escape.Sequence, ShellChars.DoubleQuote.Sequence, "$"];

        if (!chars.Contains(token.RawValue[1..2]))
        {
            expansion.ExpandedValue = expansion.RawValue;
            
        }
        
        expansion.IsQuoted = false;

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
        ShellToken expansion = new(TokenType.Expansion);

        if (((IShellToken)token).IsQuoted)
        {
            expansion.RawValue = token.RawValue[0..1];
            expansion.ExpandedValue = expansion.RawValue;

            return expansion;

        }
        
        string input = token.RawValue;

        char quoteChar = ShellChars.SingleQuote.Sequence[0];
        int end = input.IndexOf(quoteChar, 1) >= 1 ? input.IndexOf(quoteChar, 1) : input.Length;

        expansion.RawValue = input[0..(end + 1 > input.Length ? input.Length : end + 1)];
        expansion.ExpandedValue = input[1..end];
        
        return expansion;

    }

    public static IToken ExpandDoubleQuote(IToken token)
    {
        int i;
        IShellToken expansion = new ShellToken(TokenType.Expansion);

        if (((IShellToken)token).IsQuoted)
        {
            expansion.RawValue = token.RawValue[0..1];
            expansion.ExpandedValue = expansion.RawValue;

            return expansion;

        }
        
        for (i = 1; i < token.RawValue.Length; i++)
        {
            if (token.RawValue[i] == ShellChars.DoubleQuote.Sequence[0] && !expansion.ExpandedValue.EndsWith(ShellChars.Escape.Sequence))
            {
                break;
                
            }

            expansion.ExpandedValue += token.RawValue[i];

        }

        expansion.RawValue = token.RawValue[0..(i < token.RawValue.Length ? i + 1 : token.RawValue.Length)];

        IShellToken temp = new ShellToken(TokenType.Expansion)
        {
            RawValue = expansion.ExpandedValue,
            ExpandedValue = string.Empty,
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
