using Interfaces;

namespace Shell.Extensions.ShellInputHandler;

public class ShellInputHandler : IShellInputHandler
{
    public ShellInputHandler(ILexer lexer, IExpander expander, IParser parser, IDictionary<string, IInputMap> inputMap)
    {
        Lexer = lexer;
        Expander = expander;
        Parser = parser;

        foreach (string key in inputMap?.Keys ?? [])
        {
            if (inputMap?[key].ExpansionMethod != null)
            {
                Expander.ExpansionMap.Add(key, inputMap[key].ExpansionMethod!);

            }

            if (inputMap?[key].State is not IState state)
            {
                continue;

            }

            Lexer.Controller.StateMap.Add(key, state);

            if (inputMap?[key].Token is Func<IToken> token)
            {
                Lexer.Controller.TokenMap.Add(state, token);

            }
            
        }
        
    }

    public ILexer Lexer { get; set; }

    public IExpander Expander { get; set; }

    public IParser Parser { get; set;}

    public ITree HandleInput(string input)
    {        
        Queue<IToken> tokenizedInput = Lexer?.Tokenize(input) ?? [];

        tokenizedInput = Expander?.Expand(tokenizedInput) ?? tokenizedInput;

        return Parser.Parse(tokenizedInput);

    }

    #region Structs
    public struct InputMap : IInputMap
    {
        public InputMap(IState? state = null, Func<IToken>? token = null, Func<string, string>? expansionMethod = null)
        {
            State = state;
            Token = token;
            ExpansionMethod = expansionMethod;
            
        }

        public IState? State { get; set; }

        public Func<IToken>? Token { get; set; }

        public Func<string, string>? ExpansionMethod { get; set; }
        
    }

    #endregion

}