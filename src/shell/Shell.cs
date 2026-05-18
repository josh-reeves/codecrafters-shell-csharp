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

        Builtins = new Dictionary<string, Func<IShellCommand>>()
        {
            {"echo", () => new Echo(this)},
            {"pwd", () => new PrintWorkingDirectory(this)},
            {"cd", () => new ChangeDirectory(this)},
            {"exit", () => new Exit(this)},
            {"type", () => new Type(this)}
        
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

    public IDictionary<string, Func<IShellCommand>> Builtins { get; private set; }

    #endregion

    #region Methods
    /// <summary>
    ///  The main REPL for the shell. This will set the value of ShellIsActive 
    ///   to true or false depending on whether or not external input is 
    ///   provided. The loop will execute one time regardless of the value of 
    ///   ShellIsActive. Subsequent iterations will only take place if 
    ///   ShellIsActive is equal to true.
    /// </summary>
    /// <param name="externalInput">
    ///  Optional external input for the REPL. When this is provided, the REPL
    ///   will execute in a sort of "forked" mode: No prompt character will
    ///   appear, and the REPL will only execute once before the method returns.
    /// </param>
    public void Run(string? externalInput = null)
    {
        ShellIsActive = externalInput == null;

        do
        {
            try
            {
                ShellCommand command = new(this);

                Console.Write(ShellIsActive ? prompt : string.Empty);

                command.Execute(inputHandler.HandleInput(
                    externalInput ??
                    Console.ReadLine() ?? 
                    string.Empty).Root);

                if (command.IsStdErrRedirected)
                {
                    foreach(StreamWriter writer in ErrWriters)
                    {
                        writer.WriteLine(command.StandardError);

                    }    

                }

                Reset();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

            }

        }
        while (ShellIsActive);

    }

    /// <summary>
    /// Resets the shell's state so that it's ready to receive and interpret the
    ///  next command: Closes and disposes of any open stream writers and their
    ///  associated pipes. Waits to ensure that all forks of the shell have
    ///  exited, clears the list of forks and closes any open stream readers.
    /// </summary>
    private void Reset()
    {
        IList<StreamWriter> writers = OutWriters.Concat(ErrWriters).ToList();

        foreach (StreamWriter writer in writers)
        {
            writer.Close();

        }

        foreach (Process fork in Forks)
        {
            fork.WaitForExit();
            fork.Close();
        }

        InReader?.Dispose();

        Forks.Clear();
        OutWriters.Clear();
        ErrWriters.Clear();

    }

    /// <summary>
    ///  Determines whether or not any of a provided list of files is executable.
    /// </summary>
    /// <param name="files">
    ///  An array of strings representing paths of files to check.
    /// </param>
    /// <returns>
    ///  True if any of the provided files are executable. Otherwise false.
    /// </returns>
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

    /// <summary>
    ///  Searches for a given file name in a given enumerable of directorys and
    ///   and return any located instances of the file.
    /// </summary>
    /// <param name="file">
    ///  The file name to search for.
    /// </param>
    /// <param name="directories">
    ///  An enumerable of directories to search.
    /// </param>
    /// <returns>
    ///  A list of directories containing the given file name.
    /// </returns>
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

    