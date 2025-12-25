namespace Interfaces;

public interface IShellEnvironment
{
    public bool ShellIsActive { get; set; }

    public char CommandSeparator { get; }

    public char HomeChar { get; }

    public char PathSeparator { get;}  

    public string PathVar { get; }

    public string Path { get; }

    public string HomeDir { get; }

    public IParser Parser { get; }

    public IStateController? Controller { get; }

    public IDictionary<string, IState>? ControlSequences { get; }

    public IList<string> PathList { get; }

    public IDictionary<string, IShellCommand> Commands { get; }

    public void ExecuteExternal(string executable, string[] args);

    public bool IsExecutable(string[] files);

    public IEnumerable<string> Search(string file, IEnumerable<string> directories);

}