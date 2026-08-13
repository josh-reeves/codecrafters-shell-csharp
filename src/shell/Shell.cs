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
    private readonly int historyCap;
    private readonly string prompt,
                            historyFile;
    
    private IShellReader reader => inputHandler.Reader;
    private readonly ShellControls controls;
    private readonly IShellInputHandler inputHandler;       

    #endregion

    #region Constructor(s)
    public Shell(int historyCapacity, string historyFilePath, string promptSeq, string pathVar, char commandSeparator, IShellInputHandler shellInputHandler)
    {

        historyCap = historyCapacity >= 0 ? historyCapacity : 0;
        prompt = promptSeq;
        historyFile = historyFilePath;
        inputHandler = shellInputHandler;
        
        if (File.Exists(historyFile))
        {
            InputHistory = [..File.ReadAllLines(historyFile)];

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

        controls = new(this, inputHandler.Reader);

        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.Enter, false, false, false), controls.Enter);
        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.Backspace, false, false, false), controls.Backspace);
        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false), controls.RetrieveHistoryEntry);
        reader.KeyMap.Add(new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false), controls.RetrieveHistoryEntry);

        inputHandler.Reader.Prompt = prompt;

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
        
        Debugger?.WriteLine($"REPL: Launching Shell. Interactive mode: {ShellIsActive}");
        
        do
        {
            try
            {
                ShellCommand command = new(this)
                {
                    Debugger = Debugger

                };

                InputHistory.Add(string.Empty);

                controls.HistoryIndex = InputHistory.Count - 1;

                InputHistory[InputHistory.Count - 1] = externalInput ?? inputHandler.Reader.Read(prompt) ?? string.Empty;
                
                if (string.IsNullOrWhiteSpace(InputHistory[InputHistory.Count - 1]))
                {
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
                Debugger?.WriteLine($"EXCEPTION: {ex.Message}");

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
            Debugger?.WriteLine($"REPL: Redirecting {output} to {writers.Count()} StreamWriters.");

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

        for (int i = InputHistory.Count >= historyCap ? InputHistory.Count - 1 - historyCap : 0; i <= InputHistory.Count - 1; i ++)
        {
            trucatedHistory.Add(InputHistory[i]);
        
        }

        File.WriteAllLines(historyFile, trucatedHistory);

    }

    #endregion

    #region Classes & Structs
    private class ShellControls
    {
        private IList<string> history => Shell.InputHistory;

        public ShellControls(IShell shell, IShellReader reader)
        {
            Shell = shell;
            Reader = reader;
            
        }

        #region Properties
        public int HistoryIndex { get; set; }

        public IShell Shell { get; set; }

        public IShellReader Reader { get; set; }

        #endregion

        #region Methods
        public string Enter(string input, ConsoleKeyInfo keyInfo)
        {
            Reader.Active = false;
            
            return input;

        }

        public string Backspace(string input, ConsoleKeyInfo info)
        {
            if (input.Length > 0)
            {
                input = input.Remove(input.Length - 1);
                Console.Write("\b \b");
            
            }

            return input;

        }

        public string RetrieveHistoryEntry(string input, ConsoleKeyInfo keyInfo)
        {
            if (keyInfo.Key == ConsoleKey.UpArrow && HistoryIndex == history.Count - 1)
            {
                history[history.Count - 1] = input;
                
            }

            if (keyInfo.Key == ConsoleKey.UpArrow && HistoryIndex > 0)
            {
                HistoryIndex--;

                input = history[HistoryIndex];            

            }

            if (keyInfo.Key == ConsoleKey.DownArrow && HistoryIndex < history.Count - 1)
            {
                HistoryIndex++;

                input = history[HistoryIndex];
                
            }

            Reader.ClearLine(Reader.Prompt.Length);

            Console.Write(input);

            return input;
            
        }

        #endregion
    
    }

    #endregion

}

    