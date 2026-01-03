using Interfaces;

namespace Shell.Extensions.Parser.Tokens;

public class RedirectStdOutToken : IToken
{
    #region Constructor(s)
    public RedirectStdOutToken()
    {
        Value = string.Empty;
        ExpandedValue = string.Empty;

    }

    #endregion

    #region Properties
    public string Value { get; set; }

    public string ExpandedValue { get; set; }
    
    #endregion
}
