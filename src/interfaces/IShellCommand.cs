namespace Interfaces;

public interface IShellCommand
{
    #region Properties
    public bool IsStdInRedirected { get; }

    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public StreamReader StandardOutput { get; }

    public StreamReader StandardError { get; }

    #endregion

    #region Methods
    public void Execute(object? args);

    #endregion

}