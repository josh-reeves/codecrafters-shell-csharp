using Interfaces;

namespace Shell.Core.Input.ShellInputHandler.Expander;

public class Expander : IExpander, IDebuggable
{
    #region Constructor(s)
    public Expander()
    {
        ExpansionMap = new Dictionary<string, Func<IToken, IToken>>();
        ExpansionMethods = [];
        
    }

    #endregion

    #region Properties
    public IDebugger? Debugger { get; set; }

    public IDictionary<string, Func<IToken, IToken>> ExpansionMap { get; }

    public IList<Func<string, string>> ExpansionMethods { get; }

    #endregion

    #region Methods
    public Queue<IToken> Expand(Queue<IToken> tokens)
    {
#if DEBUG
    Debugger?.WriteLine($"EXPANSION: Beginning expansion...");
#endif
        foreach (IToken token in tokens)
        {
            token.ExpandedValue = Expand(token).ExpandedValue;

        }
#if DEBUG
    Debugger?.WriteLine($"EXPANSION: Expansion complete.");
#endif
        return tokens;

    }

    public IToken Expand(IToken token)
    {
        token.ExpandedValue = token.RawValue;
#if DEBUG
        Debugger?.WriteLine($"EXPANSION: New Token - Raw Value: {token.RawValue}");
#endif
        for (int i = 0; i < token.ExpandedValue.Length; i++)
        {
            string remaining = token.ExpandedValue[i..];
            IToken? expansion = null;
#if DEBUG
            Debugger?.WriteLine($"EXPANSION: Remaining: {remaining}");
#endif
            foreach (string key in ExpansionMap.Keys)
            {
#if DEBUG                
                Debugger?.WriteLine($"EXPANSION: Checking for match: {key}");
#endif
                if (remaining.StartsWith(key))
                {
#if DEBUG
                    Debugger?.WriteLine($"EXPANSION: Token match: {key}");
#endif
                    /* Grabs a token from the first expansion method without
                     *  requiring a hard depedency to the token class to be
                     *  created. The value does not matter:*/
                    expansion = ExpansionMap.First().Value(token);
                    expansion.ExpandedValue = expansion.RawValue = remaining;
                    ((IShellToken)expansion).IsQuoted = ((IShellToken)token).IsQuoted;
                    
                    expansion = ExpansionMap[key](expansion);
#if DEBUG
                    Debugger?.WriteLine($"EXPANSION: New Expansion - Raw Value: {expansion.RawValue}, Expanded Value: {expansion.ExpandedValue}");
#endif
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
