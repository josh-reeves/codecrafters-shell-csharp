using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    #region Constructor(s)
    public Expander()
    {
        ExpansionMap = new Dictionary<string, Func<IToken, IToken>>();
        
    }

    #endregion

    #region Properties
    public IDictionary<string, Func<IToken, IToken>> ExpansionMap { get; }

    #endregion

    #region Methods
    public Queue<IToken> Expand(Queue<IToken> tokens)
    {
        foreach (IToken token in tokens)
        {
            token.ExpandedValue = Expand(token).ExpandedValue;

        }
        
        return tokens;

    }

    public IToken Expand(IToken token)
    {
        token.ExpandedValue = token.RawValue;

        for (int i = 0; i < token.ExpandedValue.Length; i++)
        {
            string remaining = token.ExpandedValue[i..];
            IToken? expansion = null;

            foreach (string key in ExpansionMap.Keys)
            {
                if (remaining.StartsWith(key))
                {
                    expansion = ExpansionMap[key](token);

                    token.ExpandedValue = token.ExpandedValue.Remove(i, expansion.RawValue.Length).Insert(i, expansion.ExpandedValue);

                    remaining = token.ExpandedValue[i..];        
        
                }
                
            }
        
            if (expansion is not null)
            {
                i += (expansion.ExpandedValue.Length - 1 > 0) ? expansion.ExpandedValue.Length - 1 : 0;

            }

        }

        return token;

    }

    #endregion

}
