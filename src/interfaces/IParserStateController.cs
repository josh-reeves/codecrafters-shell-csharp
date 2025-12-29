using System;

namespace Interfaces;

public interface IParserStateController : IStateController
{
#region Properties
    public string RemainingText { get; set; }

    public IToken? CurrentToken { get; set; }

    public IParser Parser { get; }

    public IList<IToken> ParsedTokens { get; }

#endregion

}
