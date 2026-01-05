using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    public Expander()
    {
        GroupDelimiters = [];
        
    }

    public IList<char> GroupDelimiters { get; private set; }

    public IList<IToken> Expand(IList<IToken> tokens)
    {

        // Quote removal:
        foreach (IToken token in tokens)
        {
            token.ExpandedValue = token.RawValue.Trim(GroupDelimiters.ToArray());

        }
        
        return tokens;

    }

}
