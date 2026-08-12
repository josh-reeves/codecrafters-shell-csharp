using Interfaces;

namespace Shell.Extensions.Debugger;

public class Debugger : IDebugger
{
    private string file;
    
    private StreamWriter writer;

    #region Constructor(s)
    public Debugger()
    {
        Prefix = string.Empty;
        Suffix = string.Empty;
        file = string.Empty;
        writer = new StreamWriter(Console.OpenStandardOutput()) {AutoFlush = true};

    }

    #endregion

    #region Properties
    public string Prefix { get; set; }

    public string Suffix { get; set;}

    public string File
    {
        get => file;
        
        set
        {
            file = value;
            writer.Dispose();

            if (string.IsNullOrWhiteSpace(file))
            {
                writer = new(Console.OpenStandardOutput()) {AutoFlush = true};
                
                return;

            }

            writer = new(new FileStream(file, FileMode.Append, FileAccess.Write)) {AutoFlush = true};

        }

    }

    #endregion

    #region Methods
    public void Write(string msg, string[]? tags = null) 
    {
        string tagStr = "TAGS: " + string.Join(',', tags ?? []) + ' ';

        writer.Write(Prefix + tagStr + msg + Suffix);
    
    }

    public void WriteLine(string msg, string[]? tags = null)
    {
        Write(msg, tags);
        writer.WriteLine();

    }

    #endregion

}