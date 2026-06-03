namespace Interfaces;

public interface IDebugger
{
    #region Properties
    public string Prefix { get; set; }

    public string Suffix { get; set;}

    #endregion

    #region Methods
    public void Write(string msg);

    public void WriteLine(string msg);

    #endregion


}

public interface IDebuggable
{
    public IDebugger? Debugger { get; set; }

}