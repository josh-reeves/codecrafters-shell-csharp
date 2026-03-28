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
        const char separator = ' ',
                   escape = '\\',
                   homeDir = '~',
                   doubleQuote = '"',
                   singleQuote = '\'';
 
        IState defaultState = new LexerDefaultState();
        ILexerStateController stateController = new LexerStateController(
            defaultState, 
            new Dictionary<IState, Func<IToken>> 
            {
                { defaultState, () => new ShellToken(TokenType.Word) }
                
            });
        
        IShellInputHandler inputHandler = new ShellInputHandler(
            new Lexer(stateController),
            new Expander(),
            new Parser(),
            new Dictionary<string, IInputMap>
            {
                { separator.ToString(), new ShellInputHandler.InputMap(new LexerSeparatorState()) },
                { escape.ToString(), new ShellInputHandler.InputMap(new LexerEscapeState(), () => new ShellToken(TokenType.Word), ExpandEscape) },
                { singleQuote.ToString(), new ShellInputHandler.InputMap(new LexerGroupDelimiterState(singleQuote), () => new ShellToken(TokenType.Word), input => (input[0..1], string.Empty)) },
                { doubleQuote.ToString(), new ShellInputHandler.InputMap(new LexerGroupDelimiterState(doubleQuote), () => new ShellToken(TokenType.Word), input => (input[0..1], string.Empty)) },
                { ">", new ShellInputHandler.InputMap(new LexerOperatorState(">"), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "1>", new ShellInputHandler.InputMap(new LexerOperatorState("1>"), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "2>", new ShellInputHandler.InputMap(new LexerOperatorState("2>"), () => new ShellToken(TokenType.RedirectStdErr)) },
                { ">>", new ShellInputHandler.InputMap(new LexerOperatorState(">>"), () => new ShellToken(TokenType.AppendStdOut)) },
                { "1>>", new ShellInputHandler.InputMap(new LexerOperatorState("1>>"), () => new ShellToken(TokenType.AppendStdOut)) },
                { "2>>", new ShellInputHandler.InputMap(new LexerOperatorState("2>>"), () => new ShellToken(TokenType.AppendStdErr)) },
            
            });

        Shell shell = new("$ ", "PATH", separator, homeDir, inputHandler);

        shell.Run();

        (string original, string expansion) ExpandEscape(string input)
        {
            int index = input.IndexOf(escape);

            return (input[index..(index + 2)], input[index + 1].ToString());

        }
        
    }
}
