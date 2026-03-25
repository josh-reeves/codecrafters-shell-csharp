using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Expander;

public class Expander : IExpander
{
    #region Constructor(s)
    public Expander()
    {
        ExpansionMap = new Dictionary<string, Func<string, string>>();
        
    }

    #endregion

    #region Properties
    public IDictionary<string, Func<string, string>> ExpansionMap { get; }

    #endregion

    #region Methods
    public Queue<IToken> Expand(Queue<IToken> tokens)
    {
        foreach (IToken token in tokens)
        {
            token.ExpandedValue = token.RawValue;

            token.ExpandedValue = ExpandValue(token.ExpandedValue);

        }
        
        return tokens;

    }

    private string ExpandValue(string input)
    {
        string result = input;

        IEnumerable<string> keys = ExpansionMap.Keys.ToList();

        foreach (string key in ExpansionMap.Keys)
        {
            if (result.Contains(key))
            {
                result = ExpansionMap[key](result);

            }
            
        }

        return result;
        
    }

    #endregion

}

