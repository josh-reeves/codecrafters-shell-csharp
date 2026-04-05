namespace Interfaces;

public interface IInputHandler
{
    #region Properties
    public ILexer Lexer { get; }

    public IExpander Expander { get; }

    public IParser Parser { get; }

    #endregion

    #region Methods
    public ITree HandleInput(string input);

    #endregion
}
