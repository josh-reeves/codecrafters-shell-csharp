namespace Interfaces;

public interface IShell
{
    public string Command { get; set; }

    public string Filename { get; set; }

    public IShellEnvironment ShellEnvironment { get; }

    public IParser Parser { get; }

    public StreamWriter? OutputWriter { get; set; }

    public IList<string> Args { get; set; }

    public void Run();

}
