namespace Interfaces;

public interface IShellCommand
{
    public string StandardOutput { get; }

    public string StandardError { get; }

    public void Execute(object[]? args);

}