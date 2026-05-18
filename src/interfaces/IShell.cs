using System.Diagnostics;

namespace Interfaces;

public interface IShell
{
    #region Properties
    public bool ShellIsActive { get; set; }

    public char CommandSeparator { get; }

    public string PathVar { get; }

    public string Path { get; }

    public string HomeDir { get; }

    public StreamReader? InReader { get; set; }

    public IList<string> PathList { get; }

    public IList<Process> Forks { get; }

    public IList<StreamWriter> OutWriters { get; }
    
    public IList<StreamWriter> ErrWriters { get; }

    public IDictionary<string, Func<IShellCommand>> Builtins { get; }

    #endregion

    #region Methods
    public void Run(string? externalInput);

    public bool IsExecutable(string[] files);

    public IEnumerable<string> Search(string file, IEnumerable<string> directories);

    #endregion

}
