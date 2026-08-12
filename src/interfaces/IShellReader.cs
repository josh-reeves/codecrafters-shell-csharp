namespace Interfaces;

public interface IShellReader
{
    #region Properties
    public bool Active { get; set; }

    public string Prompt { get; set; }

    public IDictionary<ConsoleKeyInfo, Func<string, ConsoleKeyInfo, string>> KeyMap { get; }
    
    #endregion

    #region Events
    public event EventHandler<ConsoleKeyInfo> InputReceived;

    #endregion
    
    #region Methods
    public string Read(string prompt = "");

    public void ClearLine(int startPos = 0)
    {
        
    }

    #endregion

}