namespace Interfaces;

public interface IShellCommand
{
    #region Properties
    public bool IsStdOutRedirected { get; set; }

    public bool IsStdErrRedirected { get; set; }

    public string StandardOutput { get; }

    public string StandardError { get; }

    #endregion

    #region Methods
    public void Execute(object? args);

    #endregion

}