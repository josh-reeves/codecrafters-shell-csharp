using Interfaces;
using Shell.Extensions.ShellInputHandler.Lexer.State;

namespace Shell.Extensions.ShellInputHandler;

public class ShellInputHandler : IShellInputHandler
{
    public ShellInputHandler(ILexer shellLexer, IExpander shellExpander)
    {
        Lexer = shellLexer;
        Expander = shellExpander;
        
    }

    public ILexer Lexer { get; private set; }

    public IExpander Expander { get; private set; }

    public IList<IToken> ReadInput(string input)
    {
        IList<IToken> tokenizedInput = Lexer.Tokenize(input, new LexerStateController(Lexer, new LexerDefaultState()));

        tokenizedInput = Expander.Expand(tokenizedInput);

        List<string> processedInput = [];
        
        foreach (IToken token in tokenizedInput)
        {
            processedInput.Add(token.ExpandedValue);

        }

        return tokenizedInput;

    }

}