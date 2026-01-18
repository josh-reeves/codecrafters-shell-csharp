namespace Interfaces;

public interface IShellCommand
{
    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public string StandardOutput { get; }

    public string StandardError { get; }

    public void Execute(object? args);

}