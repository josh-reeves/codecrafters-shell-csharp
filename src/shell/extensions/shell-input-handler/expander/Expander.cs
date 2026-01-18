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

        // Quote removal:
        foreach (IToken token in tokens)
        {
            token.ExpandedValue = token.RawValue;

            foreach (char delimiter in GroupDelimiters)
            {
                token.ExpandedValue = token.ExpandedValue.Replace(delimiter.ToString(), string.Empty);

            }

        }
        
        return tokens;

    }

}

