using System.Diagnostics;
using Interfaces;
using Shell.Commands;
using Type = Shell.Commands.Type;

namespace Shell;

public class Shell : IShell
{
    #region Fields
    private string prompt;

    private IInputHandler inputHandler;
    
    #endregion

    #region Constructor(s)
    public Shell(string promptSeq, string pathVar, char commandSeparator, IInputHandler shellInputHandler)
    {
        prompt = promptSeq;

        Forks = [];
        OutWriters = [];
        ErrWriters = [];

        PathVar = pathVar;
        CommandSeparator = commandSeparator;
        inputHandler = shellInputHandler;

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
    
    public char PathSeparator { get => System.IO.Path.PathSeparator; } 

    public string PathVar { get; private set; }

    public string Path { get => Environment.GetEnvironmentVariable(PathVar) ?? string.Empty; }

    public string HomeDir { get => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }

    public StreamReader? InReader { get; set; }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IList<Process> Forks { get; }

    public IList<StreamWriter> OutWriters { get; set;}

    public IList<StreamWriter> ErrWriters { get; set; }

    public IDictionary<string, IShellCommand> Builtins { get; private set; }

    #endregion

    #region Methods
    public void Run(string? externalInput = null)
    {
        ShellIsActive = true;

        while (ShellIsActive)
        {
            try
            {
                ShellCommand command = new(this);

                if (externalInput is null)
                {
                    Console.Write(prompt);
                    
                }

                command.Execute(inputHandler.HandleInput(
                    externalInput ??
                    Console.ReadLine() ?? 
                    string.Empty).Root);

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

                foreach(Process fork in Forks)
                {
                    fork.WaitForExit();
                    fork.Close();

                }

                if (externalInput is not null)
                {
                    return;

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }

            Reset();

        }

    }

    private void Reset()
    {
        IList<StreamWriter> writers = OutWriters.Concat(ErrWriters).ToList();

        foreach (StreamWriter writer in writers)
        {
            writer.Close();

        }

        InReader?.Dispose();
        Forks.Clear();
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

    