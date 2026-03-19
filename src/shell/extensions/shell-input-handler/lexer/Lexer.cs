using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer;

public class Lexer : ILexer
{
    public Lexer(ILexerStateController controller)
    {
        RawInput = string.Empty;
        Controller = controller;
        
    }

    public string RawInput { get; private set; }

    public ILexerStateController Controller { get; }

    public Queue<IToken> TokenizedInput { get => Controller.TokenizedInput; }

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

}
