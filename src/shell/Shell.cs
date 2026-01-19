using Interfaces;
using Shell.Commands;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
    #region Fields
    private char prompt;

    private IShellInputHandler inputHandler;
    
    #endregion

    #region Constructor(s)
    public Shell(char promptChar, string pathVar, char commandSeparator, char homeChar, IShellInputHandler shellInputHandler)
    {
        prompt = promptChar;

        OutWriters = [];
        ErrWriters = [];

        PathVar = pathVar;
        CommandSeparator = commandSeparator;
        HomeChar = homeChar;
        inputHandler = shellInputHandler;

        inputHandler.Lexer.Separators.Add(CommandSeparator);
        inputHandler.Lexer.GroupDelimiters.Add('\'', new LexerGroupDelimiterState('\''));
        inputHandler.Lexer.GroupDelimiters.Add('"', new LexerGroupDelimiterState('"'));
        inputHandler.Lexer.Operators.Add(">", new LexerRedirectStdOutState(">"));
        inputHandler.Lexer.Operators.Add("1>", new LexerRedirectStdOutState("1>"));
        inputHandler.Lexer.Operators.Add("2>", new LexerRedirectStdErrState("2>"));
        inputHandler.Lexer.Operators.Add(">>", new LexerAppendStdOutState(">>"));
        inputHandler.Lexer.Operators.Add("1>>", new LexerAppendStdOutState("1>>"));
        inputHandler.Lexer.Operators.Add("2>>", new LexerAppendStdErrState("2>>"));
        
        foreach (char key in inputHandler.Lexer.GroupDelimiters.Keys)
        {
            inputHandler.Expander.GroupDelimiters.Add(key);

        }

        Builtins = new Dictionary<string, IShellCommand>()
        {
            {"echo", new Echo(this)},
            {"pwd", new PrintWorkingDirectory(this)},
            {"cd", new ChangeDirectory(this)},
            {"exit", new Exit(this)},
            {"type", new Type(this)}
       
        };

    }

    #endregion

    #region Properties
    public bool ShellIsActive { get; set; }

    public char CommandSeparator { get; private set; }
    
    public char HomeChar { get; private set; }

    public char PathSeparator { get => System.IO.Path.PathSeparator; } 

    public string PathVar { get; private set; }

    public string Path { get => Environment.GetEnvironmentVariable(PathVar) ?? string.Empty; }

    public string HomeDir { get => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IList<StreamWriter> OutWriters { get; set;}

    public IList<StreamWriter> ErrWriters { get; set; }

    public IDictionary<string, IShellCommand> Builtins { get; private set; }

    #endregion

    #region Methods
    public void Run()
    {
        ShellIsActive = true;

        while (ShellIsActive)
        {
            try
            {
                Reset();

                Console.Write(prompt);

                ShellCommand command = new(this);

                command.Execute(inputHandler.ReadInput(Console.ReadLine() ?? string.Empty));

                if (command.IsStdOutRedirected)
                {
                    string output = command.StandardOutput;

                    foreach(StreamWriter writer in OutWriters)
                    {
                        writer.Write(output);
                        
                    }

                }

                if (command.IsStdErrRedirected)
                {
                    string error = command.StandardError;

                    foreach(StreamWriter writer in ErrWriters)
                    {
                        writer.Write(error);
                        
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }

        }

    }

    private void Reset()
    {
        IList<StreamWriter> writers = OutWriters.Concat(ErrWriters).ToList();

        foreach (StreamWriter writer in writers)
        {
            writer.Close();

        }

        OutWriters.Clear();
        ErrWriters.Clear();

    }

    public bool IsExecutable(string[] files)
    {
        foreach(string file in files)
        {
            if (!OperatingSystem.IsWindows())
            {
                string fileMode = File.GetUnixFileMode(file).ToString().ToLower();

                if (fileMode.Contains("execute"))
                {
                    return true;
                    
                }
                
            }

        }

        return false;
    
    }

    public IEnumerable<string> Search(string file, IEnumerable<string> directories)
    {
        char dirSep = System.IO.Path.DirectorySeparatorChar;

        List<string>? results = new();

        foreach (string dir in directories)
        {
            string path = dir + dirSep + file;

            if (File.Exists(path))
            {
                results.Add(path);

            }
        
        }
        
        return results;
        
    }

    #endregion
        
}

    