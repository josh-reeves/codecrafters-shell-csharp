using System.IO.Pipes;
using Interfaces;
using Shell.Core.Input.ShellInputHandler;
using Shell.Core.Input.ShellInputHandler.Expander;
using Shell.Core.Input.ShellInputHandler.Lexer;
using Shell.Core.Input.ShellInputHandler.Lexer.State;
using Shell.Core.Input.ShellInputHandler.Parser;
using Shell.Extensions.Debugger;

namespace Shell;

/// <remarks>
/// All of the code at the top level provides the configuration for the shell.
/// </remarks>

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

class Program
{
    static async Task Main(string[] args)
    {
        string? command = null,
                streamHandle = null;

        Debugger? debugger = null;
#if DEBUG
        debugger = new()
        {
            Prefix = $"[DEBUG-PID{Environment.ProcessId}] ",
            File = "log.txt"
            
        };

        debugger.WriteLine($"Launching PID {Environment.ProcessId}.");
#endif
        // Process external arguments:
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

        ParsingMethods.Debugger = debugger;

        // Build the dependecies for the shell:
        LexerDefaultState defaultState = new();
        
        LexerStateController stateController = new(
            defaultState, 
            new Dictionary<IState, Func<IToken>> 
            {
                { defaultState, () => new ShellToken(TokenType.Word) }
                
            })
        {
            Debugger = debugger
            
        };

        ShellInputHandler inputHandler = new(new Lexer(stateController) {Debugger = debugger}, new Expander() {Debugger = debugger}, new Parser(ParsingMethods.Parse))
        {
            Debugger = debugger,
            
        };
        
        ExpansionMethods.Expander = inputHandler.Expander;

        inputHandler.RegisterInput(
            [
                ShellChars.Escape,
                ShellChars.NewLine,
                ShellChars.Command,
                ShellChars.Home,
                ShellChars.PathSeparator,
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

        // Create the shell and run the REPL::
        Shell shell = new("$ ", "PATH", ShellChars.Command.Sequence[0], inputHandler)
        {
            InReader = streamHandle is not null ? new StreamReader(new AnonymousPipeClientStream(PipeDirection.In, streamHandle)): null,
            Debugger = debugger

        };

        await shell.Run(command);

        debugger?.WriteLine($"Exiting PID {Environment.ProcessId}.");

    }

}

