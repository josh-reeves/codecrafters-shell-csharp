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
            RawValue = token.RawValue[0..2],
            ExpandedValue = token.RawValue[1..2]
            
        };

        if (((IShellToken)token).IsQuoted)
        {
            expansion.ExpandedValue = token.RawValue[0..2];
            
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

        char quoteChar = ShellChars.SingleQuote.Sequence[0];
        int end = input.IndexOf(quoteChar, 1) >= 1 ? input.IndexOf(quoteChar, 1) : input.Length;

        ShellToken expansion = new(TokenType.Expansion)
        {
            RawValue = input[0..(end + 1 > input.Length ? input.Length : end + 1)],
            ExpandedValue = input[1..end]
            
        };
        
        return expansion;

    }

    public static IToken ExpandDoubleQuote(IToken token)
    {
        int i;
        IShellToken expansion = new ShellToken(TokenType.Expansion);

        for (i = 1; i < token.RawValue.Length; i++)
        {
            Console.WriteLine($"Current char: {token.RawValue[i]}, Previous char: {token.RawValue[i - 1]}");
            if (token.RawValue[i] == ShellChars.DoubleQuote.Sequence[0] && !expansion.ExpandedValue.EndsWith(ShellChars.Escape.Sequence))
            {
                Console.WriteLine("End of double quote found.");
                break;
                
            }

            expansion.ExpandedValue += token.RawValue[i];

            Console.WriteLine($"Expanded value: {expansion.ExpandedValue}");

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
