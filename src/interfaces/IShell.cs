namespace Interfaces;

public interface IShell
{
    public bool ShellIsActive { get; set; }

    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public char CommandSeparator { get; }
    
    public char HomeChar { get;  }

    public char PathSeparator { get; } 

    public string PathVar { get; }

    public string Path { get; }

    public string HomeDir { get; }

    public string InvalidCmdMsg { get; }

    public IList<string> PathList { get; }

    public IList<StreamWriter> OutWriters { get; }
    
    public IList<StreamWriter> ErrWriters { get; }

    public IDictionary<string, IShellCommand> Commands { get; }

    public void Run();

    public bool IsExecutable(string[] files);

    public IEnumerable<string> Search(string file, IEnumerable<string> directories);

}
