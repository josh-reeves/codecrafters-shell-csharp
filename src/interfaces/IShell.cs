namespace Interfaces;

public interface IShell
{
    public bool ShellIsActive { get; set; }

    public bool IsOutputRedirected { get; set; }

    public string Command { get; set; }

    public string Filename { get; set; }

    public IParser Parser { get;}

    public StreamWriter? OutputWriter { get; set;}

    public IList<string> Args { get; set; }

    public char CommandSeparator { get; }
    
    public char HomeChar { get;  }

    public char PathSeparator { get; } 

    public string PathVar { get; }

    public string Path { get; }

    public string HomeDir { get; }

    public string InvalidCmdMsg { get; }

    public IList<string> PathList { get; }

    public IDictionary<string, IShellCommand> Commands { get; }

    public void Run();

    public void Reset();

    public void ExecuteExternal(string executable, string[] args);

    public bool IsExecutable(string[] files);

    public IEnumerable<string> Search(string file, IEnumerable<string> directories);

}
