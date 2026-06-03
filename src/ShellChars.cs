using Interfaces;
using Shell.Core.Input.ShellInputHandler.Lexer;
using Shell.Core.Input.ShellInputHandler.Lexer.State;

namespace Shell;

static class ShellChars
{
    static ShellChars() {}

    public static IInputMap Command { get; } = new InputMap(' '.ToString(), new LexerSeparatorState());

    public static IInputMap Home { get; } = new InputMap("~", expansionMethod: ExpansionMethods.ExpandHome);

    public static IInputMap PathSeparator { get; } = new InputMap(Path.PathSeparator.ToString());

    public static IInputMap Escape { get; } = new InputMap("\\", new LexerEscapeState(), () => new ShellToken(TokenType.Word), ExpansionMethods.ExpandEscape);

    public static IInputMap NewLine { get; } = new InputMap(@"\n", expansionMethod: ExpansionMethods.ExpandNewLine);

    public static IInputMap SingleQuote { get; } = new InputMap("'", new LexerGroupDelimiterState('\''), () => new ShellToken(TokenType.Word), ExpansionMethods.ExpandSingleQuote);
    
    public static IInputMap DoubleQuote { get; } = new InputMap("\"", new LexerGroupDelimiterState('"', Escape.Sequence[0]), () => new ShellToken(TokenType.Word), ExpansionMethods.ExpandDoubleQuote);
    
    public static IInputMap Redirect { get; } = new InputMap(">", new LexerOperatorState(">"), () => new ShellToken(TokenType.RedirectStdOut));
    
    public static IInputMap Pipe { get; } = new InputMap("|", new LexerOperatorState("|"), () => new ShellToken(TokenType.Pipe));
    
    public static IInputMap Append { get; } = new InputMap(">>", new LexerOperatorState(">>"), () => new ShellToken(TokenType.AppendStdOut));

    /// <summary>
    /// Provides a unified container for items needed to configure the
    ///  individual components used in the input handling process.
    /// </summary>
    public struct InputMap : IInputMap
    {
        #region Constructor(s)
        public InputMap(string sequence, IState? state = null, Func<IToken>? token = null, Func<IToken, IToken>? expansionMethod = null)
        {
            Sequence = sequence;
            State = state;
            Token = token;
            ExpansionMethod = expansionMethod;
            
        }

        #endregion

        #region Properties
        public string Sequence { get; } 

        public IState? State { get; set; }

        public Func<IToken>? Token { get; set; }

        public Func<IToken, IToken>? ExpansionMethod { get; set; }

        #endregion
        
    }

}