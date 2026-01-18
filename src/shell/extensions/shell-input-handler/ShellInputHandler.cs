using Interfaces;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Shell.Extensions.ShellInputHandler.Parser.Nodes;

namespace Shell.Extensions.ShellInputHandler;

public class ShellInputHandler : IShellInputHandler
{
    public ShellInputHandler(ILexer shellLexer, IExpander shellExpander, IParser parser)
    {
        Lexer = shellLexer;
        Expander = shellExpander;
        Parser = parser;
        
    }

    public ILexer Lexer { get; private set; }

    public IExpander Expander { get; private set; }

    public IParser Parser { get; private set;}

    public ITree ReadInput(string input)
    {
        Queue<IToken> tokenizedInput = Lexer.Tokenize(input, new LexerStateController(Lexer, new LexerDefaultState()));

        tokenizedInput = Expander.Expand(tokenizedInput);

        return (CommandTree)Parser.Parse(tokenizedInput);

    }

}