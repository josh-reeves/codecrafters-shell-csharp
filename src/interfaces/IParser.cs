using System.Collections;

namespace Interfaces;

public interface IParser
{
    public IList<char> Separators { get; set;}

    public IList<string> Operators { get; set; }

    /// <summary>
    /// Lexes input string into a series of tokens.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="separators"></param>
    /// <param name="operators"></param>
    /// <param name="groupDelimiters"></param>
    /// <returns></returns>
    public IList<IToken> Tokenize(string input, IParserStateController controller);
   
    public IEnumerator<string> Parse();
   
}