using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    #region Constructor(s)
    public Expander()
    {
        GroupDelimiters = [];
        EscapeCharacters = [];
        
    }

    #endregion

    #region Properties
    public IList<char> GroupDelimiters { get; private set; }

    public IList<char> EscapeCharacters { get; private set; }

    #endregion

    #region Methods
    public Queue<IToken> Expand(Queue<IToken> tokens)
    {
        if (tokens.Count <= 0 )
        {
            return tokens;

        }

        foreach (IToken token in tokens)
        {
            token.ExpandedValue = token.RawValue;

            token.ExpandedValue = RemoveQuotes(token.ExpandedValue);

        }
        
        return tokens;

    }

    private string RemoveQuotes(string input)
    {
        string result = input;

        foreach (char chr in result)
        {
            if (EscapeCharacters.Contains(chr))
            {
                result.Remove(result.IndexOf(chr), 1);

                continue;

            }

            if (GroupDelimiters.Contains(chr))
            {
                
            }

        }

        int delimiterIndex = result.IndexOfAny(GroupDelimiters.ToArray());

        if (delimiterIndex == -1)
        {
            return result;

        }

        char delimiter = result[delimiterIndex];

        result = result.Replace(delimiter.ToString(), string.Empty);


        return result;
        
    }

    #endregion

}

