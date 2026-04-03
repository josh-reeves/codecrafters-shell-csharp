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
        int index = 1;

        string original,
               expansion = input;

        while (index < expansion.Length && expansion[index] != chars.DoubleQuote) 
        {
            index = expansion.IndexOfAny([chars.DoubleQuote, chars.EscapeChar], index) >= 0 ? expansion.IndexOfAny([chars.DoubleQuote, chars.EscapeChar], index) : expansion.Length;

            if (expansion[index >= expansion.Length ? index - 1 : index] == chars.EscapeChar)
            {
                expansion = expansion.Remove(index, 1);

                index++;

            };

        }

        int offset = input.Length - expansion.Length;

        original = input[0..(index + offset < input.Length ? index + offset + 1 : index + offset)];
        expansion = expansion[1..index];

        return (original, expansion);

    } 

    #endregion
}
