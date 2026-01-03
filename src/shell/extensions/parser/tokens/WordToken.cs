using Interfaces;

namespace Shell.Extensions.Parser.Tokens;

public class WordToken : IToken
{
    public WordToken()
    {
        Value = string.Empty;
        ExpandedValue = string.Empty;
        
    }

    public string Value { get; set; }

    public string ExpandedValue { get; set; }

}
