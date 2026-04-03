using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer;

public class Lexer : ILexer
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

    public ILexerStateController Controller { get; }

    public Queue<IToken> TokenizedInput { get => Controller.TokenizedInput; }

    #endregion

    #region Methods
    public Queue<IToken> Tokenize(string input)
    {
        TokenizedInput.Clear();
 
        Controller.RemainingText = RawInput = input;

        while (!string.IsNullOrWhiteSpace(Controller.RemainingText))
        {
            Controller.CurrentState.Execute();

        }

        return TokenizedInput;

    }
    
    #endregion

}
