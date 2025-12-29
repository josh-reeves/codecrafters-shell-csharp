using Interfaces;
using Shell.Commands;
using Shell.Extensions;
using Shell.Extensions.Parser;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
#region Fields
    private const string invalidCmdMsg = ": command not found";
    private string command;

    private ShellEnvironment environment;

    private List<string> tokenizedInput;

#endregion

#region Constructor(s)
    public Shell()
    {
        command = string.Empty;

        environment = new ShellEnvironment(
            this, 
            new Parser([], []), 
            ' ', 
            '~', 
            "PATH");

        environment.Parser.Separators.Add(environment.CommandSeparator);

        environment.Commands.Add("exit", new Exit(environment));
        environment.Commands.Add("echo", new Echo(environment));
        environment.Commands.Add("type", new Type(environment));
        environment.Commands.Add("pwd", new PrintWorkingDirectory(environment));
        environment.Commands.Add("cd", new ChangeDirectory(environment));

        tokenizedInput = new();

        Args = new List<string>();

    }

#endregion

#region Properties
    public IList<string>? Args { get; private set;}

#endregion

#region Methods
    public void Run()
    {
        environment.ShellIsActive = true;

        while (environment.ShellIsActive)
        {
            tokenizedInput.Clear();

            Console.Write("$ ");

            string rawInput = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(rawInput))
            {
                continue;

            }

            foreach(IToken token in environment.Parser.Tokenize(rawInput, new ParserStateController(environment.Parser, new ParserDefaultState())))
            {
                tokenizedInput.Add(token.Value);
                
            };

            Console.WriteLine(tokenizedInput.Count);

            command = tokenizedInput[0];
            Args = tokenizedInput[1..tokenizedInput.Count];

            if (environment.Commands.Keys.Contains(command))
            {
                 environment.Commands[command]?.Execute(Args.ToArray());

                 continue;
               
            }

            if (environment.IsExecutable(environment.Search(command,environment.PathList).ToArray()))
            {
                environment.ExecuteExternal(command, Args.ToArray());
            
                continue;
                
            }

            Console.WriteLine(command + invalidCmdMsg);

        }

    }

#endregion
        
}

    