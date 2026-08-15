using Interfaces;
using System.Diagnostics;
using Shell.Core.Commands;
using Type = Shell.Core.Commands.Type;
using Shell.Core.Input.ShellInputHandler.Parser;
using System.Xml;

namespace Shell;

public class Shell : IShell, IDebuggable
{
    #region Fields
    private readonly int histCap;
    private readonly string histFile;
    private int histIndex;
    
    private IShellReader reader => inputHandler.Reader;
    private readonly ShellControls controls;
    private readonly IShellInputHandler inputHandler;       

    #endregion

    #region Constructor(s)
    public Shell(int historyCapacity, string historyFilePath, string pathVar, char commandSeparator, IShellInputHandler shellInputHandler)
    {

        histCap = historyCapacity >= 0 ? historyCapacity : 0;
        histFile = historyFilePath;
        inputHandler = shellInputHandler;
        
        if (File.Exists(histFile))
        {
            InputHistory = [..File.ReadAllLines(histFile)];

        }
        else
        {
            InputHistory = [];
            
        }

        Forks = [];
        OutWriters = [];
        ErrWriters = [];

        PathVar = pathVar;
        CommandSeparator = commandSeparator;

        Builtins = new Dictionary<string, Func<IShellCommand>>()
        {
            {"echo", () => new Echo(this)},
            {"pwd", () => new PrintWorkingDirectory(this)},
            {"cd", () => new ChangeDirectory(this)},
            {"exit", () => new Exit(this)},
            {"type", () => new Type(this)},
            {"history", () => new History(this)}
        
        };

        controls = new(this);

        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false), controls.Enter);
        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false), controls.Backspace);
        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), controls.UpArrow);
        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), controls.DownArrow);

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

    public IDebugger? Debugger { get; set; }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IList<string> InputHistory { get; }

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
    public async Task Run(string? externalInput = null)
    {
        ShellIsActive = externalInput == null;
        
        Debugger?.WriteLine($"Launching Shell. Interactive mode: {ShellIsActive}", ["REPL"]);
        
        do
        {
            try
            {
                ShellCommand command = new(this)
                {
                    Debugger = Debugger

                };

                InputHistory.Add(string.Empty);

                histIndex = InputHistory.Count - 1;

                InputHistory[InputHistory.Count - 1] = externalInput ?? reader.Read() ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(InputHistory[InputHistory.Count - 1]))
                {
                    InputHistory.RemoveAt(InputHistory.Count - 1);
                    
                    continue;

                }

                ITree commandTree = inputHandler.HandleInput(InputHistory[InputHistory.Count - 1]);

                command.Execute(commandTree.Root);

                if (command.IsStdOutRedirected)
                {
                    await RedirectStream(command.StandardOutput, OutWriters);

                }

                if (command.IsStdErrRedirected)
                {
                    await RedirectStream(command.StandardError, ErrWriters);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An unhandled exception occured.");
                Debugger?.WriteLine(ex.Message, ["EXCEPTION"]);

            }

            Reset();

        }
        while (ShellIsActive);

        SaveHistory();

    }

    private async Task RedirectStream(StreamReader reader, IEnumerable<StreamWriter> writers)
    {
        while (await reader.ReadLineAsync() is string output)
        {
            Debugger?.WriteLine($"Redirecting {output} to {writers.Count()} StreamWriters.", ["REPL"]);

            foreach (StreamWriter writer in writers)
            {
                await writer.WriteLineAsync(output);

            }
            
        }

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
            writer.Dispose();

        }

        foreach (Process fork in Forks)
        {
            fork.WaitForExit();
            fork.Close();
        }

        InReader?.Close();
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

    private void SaveHistory()
    {
        List<string> trucatedHistory = [];

        for (int i = InputHistory.Count >= histCap ? InputHistory.Count - 1 - histCap : 0; i <= InputHistory.Count - 1; i ++)
        {
            trucatedHistory.Add(InputHistory[i]);
        
        }

        File.WriteAllLines(histFile, trucatedHistory);

    }

    #endregion

    #region Classes & Structs
    private class ShellControls : IShellControls
    {
        private Shell shell;
        private int histIndex => shell.histIndex;
        private IList<string> history => shell.InputHistory;

        public ShellControls(IShell parent)
        {
            shell = (Shell)parent;
            Reader = shell.reader;
            
        }

        #region Properties
        public IShellReader Reader { get; set; }

        #endregion

        #region Methods
        public string Enter(string input)
        {
            Reader.IsReading = false;
            
            return input;

        }

        public string Backspace(string input)
        {
            if (input.Length > 0)
            {
                input = input.Remove(input.Length - 1);
                Console.Write("\b \b");
            
            }

            return input;

        }

        public string UpArrow(string input)
        {
            if (histIndex == history.Count - 1)
            {
                history[history.Count - 1] = input;
                
            }

            if (histIndex > 0)
            {
                shell.histIndex--;

                input = history[histIndex];            

            }

            Reader.ClearLine();
            Console.Write(input);

            return input;

        }

        public string DownArrow(string input)
        {
            if (histIndex < history.Count - 1)
            {
                shell.histIndex++;

                input = history[histIndex];
                
            }

            Reader.ClearLine();
            Console.Write(input);

            return input;
            
        }

        #endregion
    
    }

    #endregion

}

    