using Interfaces;

namespace Shell.Extensions.Parser;

public class Parser : IParser
{
    public Parser(IList<char> separators, IList<string> operators)
    {
        Separators = separators;
        Operators = operators;
        
    }

    public IList<char> Separators { get; set;}

    public IList<string> Operators { get; set; }

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