using System;
using Interfaces;

namespace Shell;

public class ExpansionMethods
{
    IShellChars chars;

    #region Constructor(s)
    public ExpansionMethods(IShellChars shellChars)
    {
        chars = shellChars;

    }

    #endregion

    #region Methods
    public (string original, string expansion) ExpandEscape(string input)
    {
        return (input[0..2], input[1].ToString());

    }

    public (string original, string expansion) ExpandSingleQuote(string input)
    {        
        int end = input.IndexOf(chars.SingleQuote, 1) >= 1 ? input.IndexOf(chars.SingleQuote, 1) : input.Length;

        string original = input[0..(end < input.Length ? end + 1 : end)],
               expansion = input[1..end];

        return (original, expansion);

    }

    public (string original, string expansion) ExpandDoubleQuote(string input)
    {
        int end = 1;

        while (end < input.Length && input[end] != chars.DoubleQuote) 
        {
            end = input.IndexOfAny([chars.DoubleQuote, chars.EscapeChar], end) >= 1 ? input.IndexOfAny([chars.DoubleQuote, chars.EscapeChar], end) : input.Length;

            if (input[end >= input.Length ? end - 1 : end] == chars.EscapeChar)
            {
                input = input.Remove(end, 1);

                end++;

            };

        }

        string original = input[0..(end < input.Length ? end + 1 : end)],
               expansion = input[1..end];

        return (original, expansion);

    }

    #endregion
}
