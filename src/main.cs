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
        char commandSeparator = ' ';
 
        IState defaultState = new LexerDefaultState();
        
        ShellInputHandler inputHandler = new ShellInputHandler(
            new Lexer(
                new LexerStateController(
                        defaultState, 
                        new Dictionary<IState, Func<IToken>> 
                        {
                            { defaultState, () => new ShellToken(TokenType.Word) }
                            
                        })),
            new Expander(),
            new Parser(),
            new Dictionary<string, IInputMap>
            {
                { commandSeparator.ToString(), new ShellInputHandler.InputMap(new LexerSeparatorState()) },
                { "\'", new ShellInputHandler.InputMap(new LexerGroupDelimiterState('\''), () => new ShellToken(TokenType.Word), (str) => str.Replace("\'", string.Empty)) },
                { "\"", new ShellInputHandler.InputMap(new LexerGroupDelimiterState('"'), () => new ShellToken(TokenType.Word), (str) => str.Replace("\"", string.Empty)) },
                { ">", new ShellInputHandler.InputMap(new LexerOperatorState(">"), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "1>", new ShellInputHandler.InputMap(new LexerOperatorState("1>"), () => new ShellToken(TokenType.RedirectStdOut)) },
                { "2>", new ShellInputHandler.InputMap(new LexerOperatorState("2>"), () => new ShellToken(TokenType.RedirectStdErr)) },
                { ">>", new ShellInputHandler.InputMap(new LexerOperatorState(">>"), () => new ShellToken(TokenType.AppendStdOut)) },
                { "1>>", new ShellInputHandler.InputMap(new LexerOperatorState("1>>"), () => new ShellToken(TokenType.AppendStdOut)) },
                { "2>>", new ShellInputHandler.InputMap(new LexerOperatorState("2>>"), () => new ShellToken(TokenType.AppendStdErr)) },
                { "\\", new ShellInputHandler.InputMap(null, null, (str) => str.Remove(str.IndexOf('\\'), 1)) }
            
            });

        Shell shell = new("$ ", "PATH", commandSeparator, '~', inputHandler);

        shell.Run();
        
    }

}
