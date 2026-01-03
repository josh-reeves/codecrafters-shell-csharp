using System.Dynamic;
using Interfaces;

namespace Shell.Extensions.Parser.Tokens;

public class FilenameToken : IToken
{
    #region Constructor(s)
    public FilenameToken()
    {
        Value = string.Empty;
        ExpandedValue = string.Empty;

    }

    #endregion

    #region Properties
    public string Value { get; set;}

    public string ExpandedValue { get; set; }

    #endregion 

}
