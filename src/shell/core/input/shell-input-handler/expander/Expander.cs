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
        Debugger?.WriteLine($"EXPANSION: Beginning expansion...");

        foreach (IToken token in tokens)
        {
            token.ExpandedValue = Expand(token).ExpandedValue;

        }

        Debugger?.WriteLine($"EXPANSION: Expansion complete.");

        return tokens;

    }

    private IToken Expand(IToken token)
    {
        token.ExpandedValue = token.RawValue;

        Debugger?.WriteLine($"EXPANSION: New Token. Raw Value: {token.RawValue}, IsQuoted: {(token as IShellToken)?.IsQuoted ?? false}");

        for (int i = 0; i < token.ExpandedValue.Length; i++)
        {
            bool isQuoted = (token as IShellToken)?.IsQuoted ?? false;
            string remaining = token.ExpandedValue[i..];
            IToken? expansion = null;

            Debugger?.WriteLine($"EXPANSION: Remaining: {remaining}");

            foreach (string key in ExpansionMap.Keys)
            {
                Debugger?.WriteLine($"EXPANSION: Checking for match: {key}");

                if (remaining.StartsWith(key))
                {
                    Debugger?.WriteLine($"EXPANSION: Token match: {key}");

                    /* Grabs a token from the first expansion method without
                     *  requiring a hard depedency to the token class to be
                     *  created. The value does not matter:*/
                    expansion = ExpansionMap.First().Value(token);

                    /* Reset token's IsQuoted value in case the above 
                     *  modifies it:*/
                    ((IShellToken)expansion).IsQuoted = isQuoted;

                    expansion.ExpandedValue = expansion.RawValue = remaining;
                    
                    expansion = ExpansionMap[key](expansion);

                    /* Preserve the initial token's IsQuoted value if it was set
                     *  to true, otherwise let the expansion methods take 
                     *  over:*/
                    ((IShellToken)expansion).IsQuoted = ((IShellToken)token).IsQuoted ? ((IShellToken)token).IsQuoted : ((IShellToken)expansion).IsQuoted;

                    Debugger?.WriteLine($"EXPANSION: New Expansion - Raw Value: {expansion.RawValue}, Expanded Value: {expansion.ExpandedValue}, IsQuoted: {((IShellToken)expansion).IsQuoted}");

                    token.ExpandedValue = token.ExpandedValue.Remove(i, expansion.RawValue.Length).Insert(i, expansion.ExpandedValue);

                    remaining = token.ExpandedValue[i..];

                    isQuoted = ((IShellToken)expansion).IsQuoted;

                }
                
            }
        
            if (expansion is not null)
            {
                i += (expansion.ExpandedValue.Length - 1 > 0) ? expansion.ExpandedValue.Length - 1 : 0;

            }

        }

        return token;

        /* This algorithm is a "beautiful" mess. It allows the tokens to be 
        *   scanned in roughly O(n*m) where n is the number of characters in the  
        *   token and m is the number of sequences in the expansion map. From a 
        *   performance perspective, this is better than scanning the string 
        *   multiple times, and it's probably better than using multiple string  
        *   replace operations, which, presumably, would also scan the string 
        *   multiple times behind the scenes.
        *
        *  On the other hand, though, it's absolutely hideous and much more
        *   difficult to work with than I would prefer. I need to work on 
        *   cleaning this up and streamlining some of the complexity if
        *   possible.*/

    }
    

    #endregion

}
