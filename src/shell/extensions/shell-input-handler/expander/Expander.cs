using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    public Expander()
    {
        GroupDelimiters = [];
        
    }

    public IList<char> GroupDelimiters { get; private set; }

    public Queue<IToken> Expand(Queue<IToken> tokens)
    {
        if (tokens.Count <= 0 )
        {
            return tokens;

        }

        foreach (IToken token in tokens)
        {
            token.ExpandedValue = token.RawValue;

            // Quote removal:
            int delimiterIndex = token.ExpandedValue.IndexOfAny(GroupDelimiters.ToArray());

            if (delimiterIndex != -1)
            {
                char delimiter = token.ExpandedValue[delimiterIndex];

                token.ExpandedValue = token.ExpandedValue.Replace(delimiter.ToString(), string.Empty);
                
            }

        }
        
        return tokens;

    }

}

