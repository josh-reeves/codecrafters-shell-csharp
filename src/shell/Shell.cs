using Interfaces;
using Shell.Commands;
using Shell.Extensions;
using Shell.Extensions.ShellState;
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
            new Parser(), 
            ' ', 
            '~', 
            "PATH", 
            new ShellStateController(this, new DefaultState()),
            new Dictionary<string, IState>()
            {
                {">", new RedirectStdOut()},
                {">>", new AppendStdOut()}

            });

        environment.Commands.Add("exit", new Exit(environment));
        environment.Commands.Add("echo", new Echo(environment));
        environment.Commands.Add("type", new Type(environment));
        environment.Commands.Add("pwd", new PrintWorkingDirectory(environment));
        environment.Commands.Add("cd", new ChangeDirectory(environment));

        environment.Controller?.Transitions.Add(new DefaultState(), [new DefaultState(), new RedirectStdOut(), new RedirectStdErr(), new AppendStdOut(), new AppendStdErr()]);
        environment.Controller?.Transitions.Add(new RedirectStdOut(), [new DefaultState(), new RedirectStdErr(), new AppendStdOut(), new AppendStdErr()]);
        environment.Controller?.Transitions.Add(new RedirectStdErr(), [new DefaultState(), new RedirectStdOut(), new AppendStdOut(), new AppendStdErr()]);
        environment.Controller?.Transitions.Add(new AppendStdOut(), [new DefaultState(), new RedirectStdOut(), new RedirectStdErr(), new AppendStdErr()]);
        environment.Controller?.Transitions.Add(new AppendStdErr(), [new DefaultState(), new RedirectStdOut(), new RedirectStdErr(), new AppendStdOut()]);

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
            Console.Write("$ ");

            string rawInput = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(rawInput))
            {
                return;

            }

            tokenizedInput = (List<string>)environment.Parser.Parse(
                rawInput, 
                [environment.CommandSeparator], 
                groupDelimiters: new Dictionary<char, char>{{'"', '"'}}); 

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

    