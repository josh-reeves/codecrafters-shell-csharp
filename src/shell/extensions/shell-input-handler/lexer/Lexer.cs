using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer;

public class Lexer : ILexer
{

    public Lexer()
    {
        Position = 0;
        RemainingText = string.Empty;
        Separators = [];
        TokenizedInput =[];
        Operators = new Dictionary<string, IState>();
        GroupDelimiters = new Dictionary<char, IState>();
        
    }

    public int Position { get; set; }

    public string RemainingText { get; set; }

    public IToken? CurrentToken { get; set; }

    public IList<char> Separators { get; set;}

    public IList<IToken> TokenizedInput { get; set; }

    public IDictionary<string, IState> Operators { get; }

    public IDictionary<char, IState> GroupDelimiters { get; }

    public IList<IToken> Tokenize(string input, ILexerStateController controller)
    {
        RemainingText = input;
        TokenizedInput.Clear();
        Position = 0;

        while (!string.IsNullOrWhiteSpace(RemainingText))
        {
            controller.CurrentState.Execute();

        }

        return TokenizedInput;

    }

}
