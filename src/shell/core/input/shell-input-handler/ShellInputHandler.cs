using Interfaces;

namespace Shell.Core.Input.ShellInputHandler;

/// <summary>
/// The ShellInputHandler class provides a unified interface for converting raw
///  input into a format that can be executed by the shell. The class acts as a
///  coordinator for this process, delegating the implementation of each part of 
///  the process to a separate component.
/// 
/// To facilitate this, the class also provides methods and structures that 
///  unify and simplify the process configuring the individual components.
/// </summary>
public class ShellInputHandler : IShellInputHandler, IDebuggable
{
    #region Constructor(s)
    public ShellInputHandler(IShellReader reader, ILexer lexer, IExpander expander, IParser parser, IList<IInputMap>? inputMaps = null)
    {
        Reader = reader;
        Lexer = lexer;
        Expander = expander;
        Parser = parser; 
        
        RegisterInput(inputMaps ?? []);

    } 

    #endregion



    #region Properites
    public IShellReader Reader { get; set; }

    /// <summary>
    /// The Lexer converts raw input into a queue of tokens that can be easily
    ///  interpreted regardless of any syntactic idiosyncrasies.
    /// </summary>
    public ILexer Lexer { get; set; }

    /// <summary>
    /// The Expander handles any special characters or sequences in the raw
    ///  input; removing, replacing or otherwise modifying them as defined
    ///  by the component.
    /// </summary>
    public IExpander Expander { get; set; }

    /// <summary>
    /// The parser converts the tokenized and expanded input into a syntax tree
    ///  that can be traversed and executed by the shell.
    /// </summary>
    public IParser Parser { get; set;}

    public IDebugger? Debugger { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Receives input and converts it into a syntax tree that can be executed
    ///  by the shell.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>A syntax tree representing the provided input.</returns>
    public ITree HandleInput(string input)
    {
        Debugger?.WriteLine($"Handling input: {input}", ["INPUT"]);

        Queue<IToken> tokenizedInput = Lexer?.Tokenize(input) ?? [];

        tokenizedInput = Expander?.Expand(tokenizedInput) ?? tokenizedInput;

        ITree parsedInput = Parser.Parse(tokenizedInput);
        
        Debugger?.WriteLine($"Input handling complete.", ["INPUT"]);
        
        return parsedInput;

    }

    /// <summary>
    /// Provides a method for simultaneously registering an input sequence with
    ///  the appropriate components of the input handling process.
    /// <param name="inputMaps">
    /// A list of input maps containing the character sequences and handling 
    ///  information to add to the input handler.
    /// </param>
    public void RegisterInput (IList<IInputMap> inputMaps)
    {
        foreach (IInputMap inputMap in inputMaps)
        {
            if (inputMap.ExpansionMethod != null)
            {
                Expander.ExpansionMap.Add(inputMap.Sequence, inputMap.ExpansionMethod);

            }

            if (inputMap.State is not IState state)
            {
                continue;

            }

            Lexer.Controller.StateMap.Add(inputMap.Sequence, state);

            if (inputMap.Token is Func<IToken> token)
            {
                Lexer.Controller.TokenMap.Add(state, token);

            }
            
        }
        
    }
    
    #endregion

}