using System.Diagnostics;
using Interfaces;
using Shell.Commands;
using Shell.Extensions.ShellInputHandler.Lexer.State;
using Shell.Extensions.ShellInputHandler.Lexer.Tokens;
using Shell.Extensions.ShellInputHandler;
using Shell.Extensions.ShellInputHandler.Expander;
using Shell.Extensions.ShellInputHandler.Lexer;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
    #region Fields
    private string command;

    private IShellInputHandler inputHandler;
    
    private IList<string> args;

    #endregion

    #region Constructor(s)
    public Shell(string pathVar, char commandSeparator, char homeChar, IShellInputHandler shellInputHandler)
    {  
        command = string.Empty;
        args = [];
        OutWriters = [];
        ErrWriters = [];

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
        inputHandler.Lexer.Operators.Add(">>", new LexerAppendStdOutState(">>"));
        inputHandler.Lexer.Operators.Add("1>>", new LexerAppendStdOutState("1>>"));
        inputHandler.Lexer.Operators.Add("2>>", new LexerAppendStdErrState("2>>"));
        
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

    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public char CommandSeparator { get; private set; }
    
    public char HomeChar { get; private set; }

    public char PathSeparator { get => System.IO.Path.PathSeparator; } 

    public string PathVar { get; private set; }

    public string Path { get => Environment.GetEnvironmentVariable(PathVar) ?? string.Empty; }

    public string HomeDir { get => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }

    public string InvalidCmdMsg { get; private set; }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IList<StreamWriter> OutWriters { get; set;}

    public IList<StreamWriter> ErrWriters { get; set; }

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
                    switch (input[i])
                    {
                        case RedirectStdOutToken:
                            i++;

                            RedirectStdOut(input[i].ExpandedValue);

                            continue;

                        case RedirectStdErrToken:
                            i++;
                            
                            RedirectStdErr(input[i].ExpandedValue);

                            continue;

                        case AppendStdOutToken:
                            i++;
                            
                            AppendStdOut(input[i].ExpandedValue);

                            continue;
                            
                        case AppendStdErrToken:
                            i++;
                            
                            AppendStdErr(input[i].ExpandedValue);

                            continue;
                            
                    }
    
                    args.Add(input[i].ExpandedValue);
                    
                }

                if (Commands.Keys.Contains(command))
                {
                    Commands[command]?.Execute(args.ToArray());

                    foreach (StreamWriter writer in OutWriters)
                    {
                        writer.Write(Commands[command].StandardOutput);

                    }

                    foreach (StreamWriter writer in ErrWriters)
                    {
                        writer.Write(Commands[command].StandardError);

                    }

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
        command = string.Empty;
        args = [];

        IList<StreamWriter> writers = OutWriters.Concat(ErrWriters).ToList();

        foreach (StreamWriter writer in writers)
        {
            writer.Close();

        }

        OutWriters.Clear();
        ErrWriters.Clear();

        Console.SetOut(
            new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
                
            });

        IsStdOutRedirected = false;
        IsStdErrRedirected = false;

    }

    private void ExecuteExternal(string executable, string[] args)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = executable,
                UseShellExecute = !IsStdOutRedirected && !IsStdErrRedirected,
                RedirectStandardOutput = IsStdOutRedirected,
                RedirectStandardError = IsStdErrRedirected

            }
            
        };

        foreach (string arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);

        }

        process.Start();

        if (IsStdOutRedirected)
        {
            string output = process.StandardOutput.ReadToEnd();
        
            foreach (StreamWriter writer in OutWriters)
            {
                writer.Write(output);

            }
            
        }

        if (IsStdErrRedirected)
        {
            string error = process.StandardError.ReadToEnd();
    
            foreach (StreamWriter writer in ErrWriters)
            {
                writer.Write(error);
                
            }
            
        }

        process.WaitForExit();

    }

    private void RedirectStdOut(string file)
    {
        OutWriters.Add(new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write))
        {
            AutoFlush = true

        });
        
        IsStdOutRedirected = true;
        
    }

    private void RedirectStdErr(string file)
    {
        ErrWriters.Add(new StreamWriter(new FileStream(file, FileMode.Create, FileAccess.Write))
        {
            AutoFlush = true

        });
        
        IsStdErrRedirected = true;

    }

    private void AppendStdOut(string file)
    {
        OutWriters.Add(new StreamWriter(new FileStream(file, FileMode.Append, FileAccess.Write))
        {
            AutoFlush = true

        });
        
        IsStdOutRedirected = true;
        
    }

    private void AppendStdErr(string file)
    {
        ErrWriters.Add(new StreamWriter(new FileStream(file, FileMode.Append, FileAccess.Write))
        {
            AutoFlush = true

        });
        
        IsStdErrRedirected = true;
        
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

    