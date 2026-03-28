using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    #region Constructor(s)
    public Expander()
    {
        ExpansionMap = new Dictionary<char, Func<string, (string, string)>>();
        
    }

    #endregion

    #region Properties
    public IDictionary<char, Func<string, (string original, string expansion)>> ExpansionMap { get; }

    #endregion

    #region Methods
    public Queue<IToken> Expand(Queue<IToken> tokens)
    {
        foreach (IToken token in tokens)
        {
            token.ExpandedValue = token.RawValue;

            token.ExpandedValue = ExpandValue(token.RawValue);

        }
        
        return tokens;

    }

    private string ExpandValue(string input)
    {
        string result = string.Empty;

        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];

            if (ExpansionMap.ContainsKey(currentChar))
            {
                (string original, string expansion) = ExpansionMap[currentChar](input[i..input.Length]);

                result += expansion;

                i += original.Length - 1;

                continue;
            
            }

            result += currentChar;            
        }

        return result;
        
    }

    #endregion

}

