namespace Interfaces;

public interface IToken
{
    #region Properties
    public int Position { get; set; }

    public string RawValue { get; set; }

    public string ExpandedValue { get; set; }

    #endregion
}
