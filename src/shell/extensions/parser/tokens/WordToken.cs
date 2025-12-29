using Interfaces;

namespace Shell.Extensions.Parser.Tokens;

public class WordToken : IToken
{
    public WordToken()
    {
        Value = string.Empty;
        
    }

    public string Value { get; set; }

}
