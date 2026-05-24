using System.IO.Pipes;
using System.Threading.Tasks;
using Interfaces;
using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Shell.Extensions.ShellInputHandler.Parser;

namespace Shell;

/// <summary>
/// Static class providing defined flags for program arguments:
/// </summary>
static class Flags
{
    static Flags()
    {
        InputFlag = "-i";

    }
    
    public static string InputFlag { get; }
    
}

static class ShellChars
{
    static ShellChars() {}

    public static IInputMap Command { get; } = new InputMap(" ", new LexerSeparatorState());

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

class Program
{
    static async Task Main(string[] args)
    {
        string? command = null,
                streamHandle = null;

        for (int i = 0; i <= args.Length - 1; i++)
        {
            if (i == 0)
            {
                command = args[i];

                continue;
            }

            if (args[i] == Flags.InputFlag && args.Length >= i + 1)
            {
                i++;

                streamHandle = args[i];
            
            }

        }

        LexerDefaultState defaultState = new();
        LexerStateController stateController = new(
            defaultState, 
            new Dictionary<IState, Func<IToken>> 
            {
                { defaultState, () => new ShellToken(TokenType.Word) }
                
            });

        ShellInputHandler inputHandler = new(new Lexer(stateController), new Expander(), new Parser(ParsingMethods.Parse));
        ExpansionMethods.Expander = inputHandler.Expander;

        inputHandler.RegisterInput(
            [
                ShellChars.Command,
                ShellChars.Home,
                ShellChars.PathSeparator,
                ShellChars.Escape,
                ShellChars.NewLine,
                ShellChars.SingleQuote,
                ShellChars.DoubleQuote,
                ShellChars.Redirect,
                ShellChars.Pipe,
                ShellChars.Append,
                new ShellChars.InputMap("1" + ShellChars.Redirect.Sequence, new LexerOperatorState("1" + ShellChars.Redirect.Sequence), () => new ShellToken(TokenType.RedirectStdOut)),
                new ShellChars.InputMap("2" + ShellChars.Redirect.Sequence, new LexerOperatorState("2" + ShellChars.Redirect.Sequence), () => new ShellToken(TokenType.RedirectStdErr)),         
                new ShellChars.InputMap("1" + ShellChars.Append.Sequence, new LexerOperatorState("1" + ShellChars.Append.Sequence), () => new ShellToken(TokenType.AppendStdOut)),
                new ShellChars.InputMap("2" + ShellChars.Append.Sequence, new LexerOperatorState("2" + ShellChars.Append.Sequence), () => new ShellToken(TokenType.AppendStdErr)),
            
            ]);

        Shell shell = new("$ ", "PATH", ShellChars.Command.Sequence[0], inputHandler)
        {
            InReader = streamHandle is not null ? new StreamReader(new AnonymousPipeClientStream(PipeDirection.In, streamHandle)): null

        };

        await shell.Run(command);

    }

}
