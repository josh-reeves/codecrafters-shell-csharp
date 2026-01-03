using Interfaces;

namespace Shell.Extensions.Parser;

public class Parser : IParser
{
    public Parser()
    {
        Separators = [];
        TokenizedInput =[];
        Operators = new Dictionary<string, IState>();
        GroupDelimiters = new Dictionary<char, IState>();
        
    }

    public IList<char> Separators { get; set;}

    public IList<IToken> TokenizedInput { get; set; }

    public IDictionary<string, IState> Operators { get; }

    public IDictionary<char, IState> GroupDelimiters { get; }

    public IList<IToken> Tokenize(string input, IParserStateController controller)
    {
        controller.RemainingText = input;

        while (!string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.CurrentState.Execute();

        }

        return controller.ParsedTokens;

    }
   
    public IEnumerator<string> Parse()
    {
        return (IEnumerator<string>)new List<string>();

    }

}