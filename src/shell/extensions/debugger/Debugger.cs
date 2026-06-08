using System.Diagnostics;
using Interfaces;

namespace Shell.Extensions.Debugger;

public class Debugger : IDebugger
{
    #region Constructor(s)
    public Debugger()
    {
        Prefix = string.Empty;
        Suffix = string.Empty;
        File = string.Empty;

    }

    #endregion

    #region Properties
    public string Prefix { get; set; }

    public string Suffix { get; set;}

    public string File { get; set; }

    #endregion

    #region Methods
    public void Write(string msg) 
    {
        Console.Write(Prefix + msg + Suffix);
    
    }

    public void WriteLine(string msg)
    {
        Write(msg);
        Console.WriteLine();

    }

    #endregion

}