namespace Interfaces;

public interface IShellChars
{
    public char CommandSeparator { get; }

    public char HomeChar { get; }

    public char PathSeparator { get; }

    public char EscapeChar { get; }

    public char SingleQuote { get; }

    public char DoubleQuote { get; }

    public char RedirectChar { get; }

    public string AppendSeq { get; }

}
