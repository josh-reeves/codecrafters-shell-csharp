using Interfaces;
using Shell.Extensions.ShellInputHandler.Parser.Nodes;

namespace Shell.Extensions.ShellInputHandler.Parser;

/// <summary>
/// Unlike the Lexer and Expander, the Parser class is not designed to be easily 
///  configurable. Due to the nature of recusrive descent parsing,
///  the only way to configure the parsing behavior would be to provide
///  a delegate for the parsing method, which would complicate the interface 
///  and take almost as much work to use as writing a new parser each time.
/// </summary>
public class Parser : IParser
{
    #region Constructor(s)
    public Parser(Func<Queue<IToken>, ITree> parsingDelegate) 
    {
        ParsingDelegate = parsingDelegate;
        
    }
    
    #endregion

    #region Properties
    Func<Queue<IToken>, ITree> ParsingDelegate  { get; }

    #endregion

    #region Methods
    public ITree Parse(Queue<IToken> tokens)
        => ParsingDelegate(tokens);

    #endregion

}
