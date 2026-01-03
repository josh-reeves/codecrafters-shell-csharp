using System.Diagnostics;
using Interfaces;
using Shell.Commands;
using Shell.Extensions.Parser;
using Shell.Extensions.Parser.State;
using Shell.State;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
    #region Fields
    private ShellStateController controller;

    #endregion

    #region Constructor(s)
    public Shell(string pathVar, char commandSeparator, char homeChar)
    {        
        controller = new(this, new ShellInputState());

        PathVar = pathVar;
        CommandSeparator = commandSeparator;
        HomeChar = homeChar;
        InvalidCmdMsg = ": command not found";
        Command = string.Empty;
        Filename = string.Empty;
        Args = [];

        Parser = new Parser();
        Parser.Separators.Add(CommandSeparator);
        Parser.GroupDelimiters.Add('\'', new ParserGroupDelimiterState('\''));
        Parser.GroupDelimiters.Add('"', new ParserGroupDelimiterState('"'));
        Parser.Operators.Add(">", new ParserRedirectStdOutState(">"));
        Parser.Operators.Add("1>", new ParserRedirectStdOutState("1>"));

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

    public bool IsOutputRedirected { get; set; }

    public string Command { get; set; }

    public string Filename { get; set; }

    public IParser Parser { get; private set;}

    public StreamWriter? OutputWriter { get; set; }

    public IList<string> Args { get; set; }

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
               controller.CurrentState.Execute();
             
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }

        }

    }

    public void Reset()
    {
        Console.SetOut(
            new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
                
            });

        Command = string.Empty;
        Filename = string.Empty;
        Args = []; 
        IsOutputRedirected = false;
        OutputWriter?.Close();
        OutputWriter = null;

    }

    public void ExecuteExternal(string executable, string[] args)
    {
        Process process = new()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = executable,
                UseShellExecute = !IsOutputRedirected,
                RedirectStandardOutput = IsOutputRedirected

            }
            
        };

        foreach (string arg in args)
        {
            process.StartInfo.ArgumentList.Add(arg);

        }

        process.Start();

        if (IsOutputRedirected)
        {
            string output = process.StandardOutput.ReadToEnd();
            OutputWriter?.Write(output);

        }

        process.WaitForExit();

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

    