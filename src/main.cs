using Interfaces;
using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Shell.Extensions.ShellInputHandler.Lexer.Tokens;
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
                    new Dictionary<IState, Func<IToken>> {{ defaultState, () => new WordToken() }})),
            new Expander(),
            new Parser());

        inputHandler.Lexer.Controller.StateMap.Add(commandSeparator.ToString(), new LexerSeparatorState());
        inputHandler.Lexer.Controller.StateMap.Add("\'", new LexerGroupDelimiterState('\''));
        inputHandler.Lexer.Controller.StateMap.Add("\"", new LexerGroupDelimiterState('"'));
        inputHandler.Lexer.Controller.StateMap.Add(">", new LexerOperatorState(">"));
        inputHandler.Lexer.Controller.StateMap.Add("1>", new LexerOperatorState("1>"));
        inputHandler.Lexer.Controller.StateMap.Add("2>", new LexerOperatorState("2>"));
        inputHandler.Lexer.Controller.StateMap.Add(">>", new LexerOperatorState(">>"));
        inputHandler.Lexer.Controller.StateMap.Add("1>>", new LexerOperatorState("1>>"));
        inputHandler.Lexer.Controller.StateMap.Add("2>>", new LexerOperatorState("2>>"));
        
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap["\'"], () => new WordToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap["\""], () => new WordToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap[">"], () => new RedirectStdOutToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap["1>"], () => new RedirectStdOutToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap["2>"], () => new RedirectStdErrToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap[">>"], () => new AppendStdOutToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap["1>>"], () => new AppendStdOutToken());
        inputHandler.Lexer.Controller.TokenMap.Add(inputHandler.Lexer.Controller.StateMap["2>>"], () => new AppendStdErrToken());

        Shell shell = new("$ ", "PATH", commandSeparator, '~', inputHandler);

        shell.Run();
        
    }

}
