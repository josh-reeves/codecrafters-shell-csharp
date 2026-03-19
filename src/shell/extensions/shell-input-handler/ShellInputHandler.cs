using Interfaces;

namespace Shell.Extensions.ShellInputHandler;

public class ShellInputHandler : IShellInputHandler
{
    public ShellInputHandler(ILexer lexer, IExpander expander, IParser parser)
    {
        Lexer = lexer;
        Expander = expander;
        Parser = parser;

        InputMap = new Dictionary<string, IInputMap>();
        
    }

    public ILexer Lexer { get; set; }

    public IExpander Expander { get; set; }

    public IParser Parser { get; set;}

    public IDictionary<string, IInputMap> InputMap { get; }

    public ITree HandleInput(string input)
    {        
        Queue<IToken> tokenizedInput = Lexer?.Tokenize(input) ?? [];

        tokenizedInput = Expander?.Expand(tokenizedInput) ?? tokenizedInput;

        return Parser.Parse(tokenizedInput);

    }

}