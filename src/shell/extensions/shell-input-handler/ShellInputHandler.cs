using Interfaces;

namespace Shell.Extensions.ShellInputHandler;

public class ShellInputHandler : IShellInputHandler
{
    #region Constructor(s)
    public ShellInputHandler(ILexer lexer, IExpander expander, IParser parser, IDictionary<string, IInputMap>? inputMap = null)
    {
        Lexer = lexer;
        Expander = expander;
        Parser = parser;

        if (inputMap != null)
        {
            RegisterInput(inputMap);

        }
    
    }

    #endregion

    #region Properites
    public ILexer Lexer { get; set; }

    public IExpander Expander { get; set; }

    public IParser Parser { get; set;}

    #endregion

    #region Methods
    public ITree HandleInput(string input)
    {        
        Queue<IToken> tokenizedInput = Lexer?.Tokenize(input) ?? [];

        tokenizedInput = Expander?.Expand(tokenizedInput) ?? tokenizedInput;

        return Parser.Parse(tokenizedInput);

    }

    public void RegisterInput (IDictionary<string, IInputMap> inputMap)
    {
        foreach (string key in inputMap?.Keys ?? [])
        {
            if (inputMap?[key].ExpansionMethod != null)
            {
                Expander.ExpansionMap.Add(key[0], inputMap[key].ExpansionMethod!);

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

    #endregion

    #region Structs
    public struct InputMap : IInputMap
    {
        public InputMap(IState? state = null, Func<IToken>? token = null, Func<string, (string, string)>? expansionMethod = null)
        {
            State = state;
            Token = token;
            ExpansionMethod = expansionMethod;
            
        }

        public IState? State { get; set; }

        public Func<IToken>? Token { get; set; }

        public Func<string, (string, string)>? ExpansionMethod { get; set; }
        
    }

    #endregion

}