using System.Diagnostics;
using Interfaces;

namespace Shell;

public class ShellEnvironment : IShellEnvironment
{
    #region Constructor(s)
    public ShellEnvironment(IShell shell, char cmdSep, char homeChar, string pathVar)
    {
        CommandSeparator = cmdSep;
        HomeChar = homeChar;
        PathVar = pathVar;
        Shell = shell;
        InvalidCmdMsg = ": command not found";
        Commands = new Dictionary<string, IShellCommand>();

    }

    #endregion

    #region Properties
    public bool ShellIsActive { get; set; }

    public bool IsOutputRedirected { get; set; }

    public char CommandSeparator { get; private set; }
    
    public char HomeChar { get; private set; }

    public char PathSeparator { get => System.IO.Path.PathSeparator; } 

    public string PathVar { get; private set; }

    public string Path { get => Environment.GetEnvironmentVariable(PathVar) ?? string.Empty; }

    public string HomeDir { get => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }

    public string InvalidCmdMsg { get; private set; }

    public IShell Shell { get; private set; }

    public IList<string> PathList { get => Path.Split(PathSeparator).ToList(); }

    public IDictionary<string, IShellCommand> Commands { get; private set; }

    #endregion

    #region Methods
    public void ExecuteExternal(string executable, string[] args)
    {
        Process process = new();
        ProcessStartInfo startInfo = new()
        {
            FileName = executable,
            Arguments = string.Join(CommandSeparator, args)
            
        };

        if (IsOutputRedirected)
        {
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

        }

        process.StartInfo = startInfo;

        process.Start();

        if (IsOutputRedirected)
        {
            string output = process.StandardOutput.ReadToEnd();
            Shell.OutputWriter?.Write(output);

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