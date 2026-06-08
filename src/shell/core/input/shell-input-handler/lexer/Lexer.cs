using Interfaces;

namespace Shell.Core.Input.ShellInputHandler.Lexer;

public class Lexer : ILexer, IDebuggable
{
    #region Constructor(s)
    public Lexer(ILexerStateController controller)
    {
        RawInput = string.Empty;
        Controller = controller;
        
    }

    #endregion

    #region Properties
    public string RawInput { get; private set; }

    public IDebugger? Debugger { get; set; }

    public ILexerStateController Controller { get; }

    public Queue<IToken> TokenizedInput { get => Controller.TokenizedInput; }

    #endregion

    #region Methods
    public Queue<IToken> Tokenize(string input)
    {

        Debugger?.WriteLine($"TOKENIZATION: Beginning tokenization...");

        TokenizedInput.Clear();
 
        Controller.RemainingText = RawInput = input;

        while (!string.IsNullOrWhiteSpace(Controller.RemainingText))
        {
            Controller.CurrentState.Execute();

        }

        Debugger?.WriteLine($"TOKENIZATION: Tokenization complete.");

        return TokenizedInput;

    }
    
    #endregion

}
