using Interfaces;
using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Shell.Extensions.ShellInputHandler.Parser;

namespace Shell;

class Program
{
    const char escape = '\\',
            doubleQuote = '"',
            singleQuote = '\'';

    static void Main()
    {
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

    }

    static (string original, string expansion) ExpandEscape(string input)
    {
        return (input[0..2], input[1].ToString());

    }

    static (string original, string expansion) ExpandSingleQuote(string input)
    {
        char quoteChar = input[0];          
        int end = input.IndexOf(quoteChar, 1) >= 1 ? input.IndexOf(quoteChar, 1) : input.Length;
        string original = input[0..(end < input.Length ? end + 1 : end)],
               expansion = input[1..end];

        return (original, expansion);

    }

    static (string original, string expansion) ExpandDoubleQuote(string input)
    {
        char quoteChar = input[0];          
        int end = 1;

        while (end < input.Length && input[end] != quoteChar) 
        {
            end = input.IndexOfAny([quoteChar, escape], end) >= 1 ? input.IndexOfAny([quoteChar, escape], end) : input.Length;

            if (input[end >= input.Length ? end - 1 : end] == escape)
            {
                input = input.Remove(end, 1);

                end++;

            };

        }

        string original = input[0..(end < input.Length ? end + 1 : end)],
               expansion = input[1..end];

        return (original, expansion);

    }

}
