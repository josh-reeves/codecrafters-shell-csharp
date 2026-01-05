namespace Interfaces;

public interface ILexer
{
    public int Position { get; set; }

    public string RemainingText { get; set; }

    public IToken? CurrentToken { get; set; }

    public IList<char> Separators { get; set;}

    public IList<IToken> TokenizedInput { get; set; }

    public IDictionary<string, IState> Operators { get; }

    public IDictionary<char, IState> GroupDelimiters { get;}

    /// <summary>
    /// Lexes input string into a series of tokens.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="separators"></param>
    /// <param name="operators"></param>
    /// <param name="groupDelimiters"></param>
    /// <returns></returns>
    public IList<IToken> Tokenize(string input, ILexerStateController controller);
      
}