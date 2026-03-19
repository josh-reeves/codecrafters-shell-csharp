namespace Interfaces;

public interface ILexer
{
    #region Properties
    public string RawInput { get; }

    public ILexerStateController Controller { get; }

    public Queue<IToken> TokenizedInput { get; }

    #endregion

    #region Methodes
    /// <summary>
    /// Lexes input string into a series of tokens.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="separators"></param>
    /// <param name="operators"></param>
    /// <param name="groupDelimiters"></param>
    /// <returns>A queue of objects implementing IToken containing raw data for expansion.</returns>
    public Queue<IToken> Tokenize(string input);

    #endregion
      
}