using Interfaces;
using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Shell.Extensions.ShellInputHandler.Parser;

namespace Shell;

class Program
{
    static void Main()
    {
        ShellChars chars = new(
            commandSeparator: ' ',
            homeChar: '~',
            pathSeparator: Path.PathSeparator,
            escapeChar: '\\',
            singleQuote: '\'',
            doubleQuote: '"',
            redirectChar: '>',
            appendSeq: ">>"
        
        );
        
        ExpansionMethods expansionMethods = new(chars);
        LexerDefaultState defaultState = new();
        LexerStateController stateController = new(
            defaultState, 
            new Dictionary<IState, Func<IToken>> 
            {
                { defaultState, () => new ShellToken(TokenType.Word) }
                
            });

        ShellInputHandler inputHandler = new(new Lexer(stateController), new Expander(), new Parser());
        Shell shell = new("$ ", "PATH", chars.CommandSeparator, inputHandler);

        inputHandler.RegisterInput(
            new Dictionary<string, IInputMap>
            {
                { chars.CommandSeparator.ToString(), new ShellInputHandler.InputMap(new LexerSeparatorState()) },
                { chars.HomeChar.ToString(), new ShellInputHandler.InputMap(expansionMethod: (input) => (input[0].ToString(), shell.HomeDir)) },
                { chars.EscapeChar.ToString(), new ShellInputHandler.InputMap(new LexerEscapeState(), () => new ShellToken(TokenType.Word), expansionMethods.ExpandEscape) },
                { chars.SingleQuote.ToString(), new ShellInputHandler.InputMap(new LexerGroupDelimiterState(chars.SingleQuote), () => new ShellToken(TokenType.Word), expansionMethods.ExpandSingleQuote) },
                { chars.DoubleQuote.ToString(), new ShellInputHandler.InputMap(new LexerGroupDelimiterState(chars.DoubleQuote, chars.EscapeChar), () => new ShellToken(TokenType.Word), expansionMethods.ExpandDoubleQuote) },
                { chars.RedirectChar.ToString(), new ShellInputHandler.InputMap(new LexerOperatorState(chars.RedirectChar.ToString()), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "1" + chars.RedirectChar, new ShellInputHandler.InputMap(new LexerOperatorState("1" + chars.RedirectChar), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "2" + chars.RedirectChar, new ShellInputHandler.InputMap(new LexerOperatorState("2" + chars.RedirectChar), () => new ShellToken(TokenType.RedirectStdErr)) },
                { chars.AppendSeq, new ShellInputHandler.InputMap(new LexerOperatorState(chars.AppendSeq), () => new ShellToken(TokenType.AppendStdOut)) },
                { "1" + chars.AppendSeq, new ShellInputHandler.InputMap(new LexerOperatorState("1" + chars.AppendSeq), () => new ShellToken(TokenType.AppendStdOut)) },
                { "2" + chars.AppendSeq, new ShellInputHandler.InputMap(new LexerOperatorState("2" + chars.AppendSeq), () => new ShellToken(TokenType.AppendStdErr)) },
            
            });

        shell.Run();

    }

}

public struct ShellChars : IShellChars
{
    public ShellChars(char commandSeparator, char homeChar, char pathSeparator, char escapeChar, char singleQuote, char doubleQuote, char redirectChar, string appendSeq)
    {
        CommandSeparator = commandSeparator;
        HomeChar = homeChar;
        PathSeparator = pathSeparator;
        EscapeChar = escapeChar;
        SingleQuote = singleQuote;
        DoubleQuote = doubleQuote;
        RedirectChar = redirectChar;
        AppendSeq = appendSeq;

    }

    public char CommandSeparator { get; }

    public char HomeChar { get; }

    public char PathSeparator { get; }

    public char EscapeChar { get; }

    public char SingleQuote { get; }

    public char DoubleQuote { get; }

    public char RedirectChar { get; }

    public string AppendSeq { get; }

}
