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
    public IExpansion ExpandEscape(string input)
        => new Expansion(input[0..(input.Length >= 3 ? 2 : input.Length)], input.Length >= 2 ? input[1].ToString() : string.Empty);


    public IExpansion ExpandSingleQuote(string input)
    {        
        int end = input.IndexOf(chars.SingleQuote, 1) >= 1 ? input.IndexOf(chars.SingleQuote, 1) : input.Length;
        
        return new Expansion(input[0..(end < input.Length ? end + 1 : end)], input[1..end]);

    }

    public IExpansion ExpandDoubleQuote(string input)
    {
        int index = 1;

        IExpansion expansion = new Expansion(input, input);

        while (index < expansion.Expanded.Length && expansion.Expanded[index] != chars.DoubleQuote) 
        {
            index = expansion.Expanded.IndexOfAny([chars.DoubleQuote, chars.EscapeChar], index) >= 0 ? expansion.Expanded.IndexOfAny([chars.DoubleQuote, chars.EscapeChar], index) : expansion.Expanded.Length;

            if (expansion.Expanded[index >= expansion.Expanded.Length ? index - 1 : index] == chars.EscapeChar)
            {
                expansion.Expanded = expansion.Expanded.Remove(index, 1);

                index++;

            };

        }

        int offset = input.Length - expansion.Expanded.Length;

        expansion.Original = input[0..(index + offset < input.Length ? index + offset + 1 : index + offset)];
        expansion.Expanded = expansion.Expanded[1..index];

        return expansion;

    } 

    #endregion
    
    #region Structs
    public struct Expansion : IExpansion
    {
        public Expansion(string original, string expansion)
        {
            Original = original;
            Expanded = expansion;
            
        }

        #region Properties
        public string Original { get; set; }

        public string Expanded { get; set; }

        #endregion

    }

    #endregion
}
