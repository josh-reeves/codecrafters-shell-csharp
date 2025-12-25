using System.Diagnostics;
using Interfaces;

namespace Shell.Extensions;

public class ShellEnvironment : IShellEnvironment
{
    public ShellEnvironment(IShell shell, IParser parser, char cmdSep, char homeChar, string pathVar, IStateController? controller = null, IDictionary<string, IState>? controlSequences = null)
    {
        CommandSeparator = cmdSep;
        HomeChar = homeChar;
        PathVar = pathVar;
        Shell = shell;
        Parser = parser;
        Controller = controller;
        ControlSequences = controlSequences;
        Commands = new Dictionary<string, IShellCommand>();

    }

    public bool ShellIsActive { get; set; }

    public char CommandSeparator { get; private set; }
    
    public char HomeChar { get; private set; }

    public char PathSeparator { get => System.IO.Path.PathSeparator; } 

    public string PathVar { get; private set; }

    public string Path { get => Environment.GetEnvironmentVariable(PathVar) ?? string.Empty; }

    public string HomeDir { get => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }

    public IShell Shell { get; private set; }

    public IParser Parser { get; private set; }

    public IStateController? Controller { get; private set; }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IDictionary<string, IState>? ControlSequences { get; private set; }

    public IDictionary<string, IShellCommand> Commands { get; private set; }

    public void ExecuteExternal(string executable, string[] args)
    {
        char cmdSep = ' ';

        Process process = new();

        process.StartInfo = new()
        {
            FileName = executable,
            Arguments = string.Join(cmdSep, args)

        };

        process.Start();

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

}