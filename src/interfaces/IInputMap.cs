namespace Interfaces;

public interface IInputMap
{
    #region Properties
    public string Sequence { get; }

    public IState? State { get; }

    public Func<IToken>? Token { get; }

    public Func<string, (string, string)>? ExpansionMethod { get; }

    #endregion

}