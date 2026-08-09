namespace Interfaces;

public interface IShellInputHandler : IInputHandler
{
    #region Properties
    public IDictionary<ConsoleKeyInfo, Func<string, string>> KeyMap { get; }
    
    #endregion

    #region Events
    public event EventHandler<ConsoleKeyInfo> InputReceived;

    #endregion
    
    #region Methods
    public string CaptureInput(ConsoleKeyInfo accept, string prompt = "");

    #endregion

}