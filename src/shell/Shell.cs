using System.Diagnostics;
using Interfaces;
using Shell.Commands;
using Shell.Extensions.Lexer.State;
using Shell.Extensions.Lexer.Tokens;
using Shell.Extensions.Parser;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
    #region Fields
    private bool isStdOutRedirected,
                 isStdErrRedirected;

    private string command,
                   stdOutFile,
                   stdErrFile;

    private StreamWriter? outWriter,
                          errWriter;
    private IShellInputHandler inputHandler;
    private IList<string> args;

    #endregion

    #region Constructor(s)
    public Shell(string pathVar, char commandSeparator, char homeChar)
    {  
        command = string.Empty;
        stdOutFile = string.Empty;
        stdErrFile = string.Empty;
        args = [];

        PathVar = pathVar;
        CommandSeparator = commandSeparator;
        HomeChar = homeChar;
        InvalidCmdMsg = ": command not found";
        inputHandler = new ShellInputHandler(new Lexer(), new Expander());

        inputHandler.Lexer.Separators.Add(CommandSeparator);
        inputHandler.Lexer.GroupDelimiters.Add('\'', new LexerGroupDelimiterState('\''));
        inputHandler.Lexer.GroupDelimiters.Add('"', new LexerGroupDelimiterState('"'));
        inputHandler.Lexer.Operators.Add(">", new LexerRedirectStdOutState(">"));
        inputHandler.Lexer.Operators.Add("1>", new LexerRedirectStdOutState("1>"));
        inputHandler.Lexer.Operators.Add("2>", new LexerRedirectStdErrState("2>"));
        
        foreach (char key in inputHandler.Lexer.GroupDelimiters.Keys)
        {
            inputHandler.Expander.GroupDelimiters.Add(key);

        }

        Commands = new Dictionary<string, IShellCommand>()
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

    public string InvalidCmdMsg { get; private set; }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IDictionary<string, IShellCommand> Commands { get; private set; }

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

                Console.Write("$ ");

                IList<IToken> input = inputHandler.ReadInput(Console.ReadLine() ?? string.Empty);

                if (input.Count <= 0)
                {
                    continue;
                    
                }

                command = input[0].ExpandedValue;
                
                for (int i = 1; i < input.Count; i++)
                {
                    if (input[i] is RedirectStdOutToken)
                    {
                        i++;

                        stdOutFile = input[i].ExpandedValue;
                        
                        RedirectStdOut();

                        continue;
                        
                    }

                    if (input[i] is RedirectStdErrToken)
                    {
                        i++;

                        stdErrFile = input[i].ExpandedValue;
                        
                        RedirectStdErr();

                        continue;
                        
                    }

                    args.Add(input[i].ExpandedValue);
                    
                }

                if (Commands.Keys.Contains(input[0].ExpandedValue))
                {
                    Commands[input[0].ExpandedValue]?.Execute(args.ToArray());

                    continue;

                }
                
                if (IsExecutable(Search(command, PathList).ToArray()))
                {
                    ExecuteExternal(command, args.ToArray());

                    continue;
                    
                }

                Console.WriteLine(command + InvalidCmdMsg);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }

        }

    }

    private void Reset()
    {
        Console.SetOut(
            new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
                
            });

        command = string.Empty;
        stdOutFile = string.Empty;
        stdErrFile = string.Empty;
        args = [];
        isStdOutRedirected = false;
        isStdErrRedirected = false;
        outWriter?.Close();
        outWriter = null;
        errWriter?.Close();
        errWriter = null;

    }

    private void ExecuteExternal(string executable, string[] args)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = executable,
                UseShellExecute = !isStdOutRedirected && !isStdErrRedirected,
                RedirectStandardOutput = isStdOutRedirected,
                RedirectStandardError = isStdErrRedirected

            }
            
        };

        foreach (string arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);

        }

        process.Start();

        if (isStdOutRedirected)
        {
            string output = process.StandardOutput.ReadToEnd();
            outWriter?.Write(output);

        }

        if (isStdErrRedirected)
        {
            string error = process.StandardError.ReadToEnd();
            errWriter?.Write(error);

        }

        process.WaitForExit();

    }

    private void RedirectStdOut()
    {
        outWriter = new StreamWriter(new FileStream(stdOutFile, FileMode.Create, FileAccess.Write))
        {
            AutoFlush = true

        };

        Console.SetOut(outWriter);

        isStdOutRedirected = true;

        
    }

    private void RedirectStdErr()
    {
        errWriter = new StreamWriter(new FileStream(stdErrFile, FileMode.Create, FileAccess.Write))
        {
            AutoFlush = true

        };

        Console.SetError(errWriter);

        isStdErrRedirected = true;
        
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

    