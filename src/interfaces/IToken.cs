namespace Interfaces;

public interface IToken
{
    public int Position { get; set; }

    public string RawValue { get; set; }

    public string ExpandedValue { get; set; }

}
