using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    #region Constructor(s)
    public Expander()
    {
        ExpansionMap = new Dictionary<string, Func<string, IExpansion>>();
        
    }

    #endregion

    #region Properties
    public IDictionary<string, Func<string, IExpansion>> ExpansionMap { get; }

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
            if (LookupKey(input[i..]) is string key)
            {
                IExpansion expansion = ExpansionMap[input[i..(i + key.Length)]](input[i..input.Length]);


                result += expansion.Expanded;

                i += expansion.Original.Length - 1;

                continue;
            
            }

            result += input[i];

        }

        return result;
        
    }

    private string? LookupKey(string input)
    {
        for(int i = ExpansionMap.Keys.MaxBy(str => str.Length)?.Length ?? 0; i > 0; i--)
        {
            string key = input.Length >= i ? input[0..i] : string.Empty;

            if (ExpansionMap.ContainsKey(key))
            {
                return key;

            }

        }

        return null;
        
    }


    #endregion

}
