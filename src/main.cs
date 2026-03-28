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
        const char escape = '\\',
                   doubleQuote = '"',
                   singleQuote = '\'';
 
        IState defaultState = new LexerDefaultState();
        ILexerStateController stateController = new LexerStateController(
            defaultState, 
            new Dictionary<IState, Func<IToken>> 
            {
                { defaultState, () => new ShellToken(TokenType.Word) }
                
            });
        
        ShellInputHandler inputHandler = new(new Lexer(stateController), new Expander(), new Parser());

        Shell shell = new("$ ", "PATH", ' ', '~', inputHandler);

        inputHandler.RegisterInput(
            new Dictionary<string, IInputMap>
            {
                { shell.CommandSeparator.ToString(), new ShellInputHandler.InputMap(new LexerSeparatorState()) },
                { shell.HomeChar.ToString(), new ShellInputHandler.InputMap(expansionMethod: (input) => (input[0].ToString(), shell.HomeDir)) },
                { escape.ToString(), new ShellInputHandler.InputMap(new LexerEscapeState(), () => new ShellToken(TokenType.Word), ExpandEscape) },
                { singleQuote.ToString(), new ShellInputHandler.InputMap(new LexerGroupDelimiterState(singleQuote), () => new ShellToken(TokenType.Word), ExpandSingleQuote) },
                { doubleQuote.ToString(), new ShellInputHandler.InputMap(new LexerGroupDelimiterState(doubleQuote, escape), () => new ShellToken(TokenType.Word), ExpandDoubleQuote) },
                { ">", new ShellInputHandler.InputMap(new LexerOperatorState(">"), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "1>", new ShellInputHandler.InputMap(new LexerOperatorState("1>"), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "2>", new ShellInputHandler.InputMap(new LexerOperatorState("2>"), () => new ShellToken(TokenType.RedirectStdErr)) },
                { ">>", new ShellInputHandler.InputMap(new LexerOperatorState(">>"), () => new ShellToken(TokenType.AppendStdOut)) },
                { "1>>", new ShellInputHandler.InputMap(new LexerOperatorState("1>>"), () => new ShellToken(TokenType.AppendStdOut)) },
                { "2>>", new ShellInputHandler.InputMap(new LexerOperatorState("2>>"), () => new ShellToken(TokenType.AppendStdErr)) },
            
            });

        shell.Run();

        // Expansion Methods --------------------------------------------------
        (string original, string expansion) ExpandEscape(string input)
        {
            return (input[0..2], input[1].ToString());

        }

        (string original, string expansion) ExpandSingleQuote(string input)
        {
            char quoteChar = input[0];          
            int end = input.IndexOf(quoteChar, 1) >= 1 ? input.IndexOf(quoteChar, 1) : input.Length;

            return (input[0..end], input[1..end]);

        }

        (string original, string expansion) ExpandDoubleQuote(string input)
        {
            char quoteChar = input[0];          
            int end = input.IndexOfAny([quoteChar, escape], 1) >= 1 ? input.IndexOfAny([quoteChar, escape], 1) : input.Length;

            return (input[0..end], input[1..end]);

        }

    }
    
}
