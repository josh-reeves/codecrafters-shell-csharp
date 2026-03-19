namespace Interfaces;

public interface IShellInputHandler
{
    #region Properties
    public ILexer Lexer { get; }

    public IExpander Expander { get; }

    public IParser Parser { get; }

    public IDictionary<string, IInputMap> InputMap { get; }

    #endregion

    #region Methods
    public ITree HandleInput(string input);

    #endregion
}
