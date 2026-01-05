namespace Interfaces;

public interface ILexerStateController : IStateController
{
#region Properties
    public ILexer Lexer { get; }

#endregion

}
