namespace Interfaces;

public interface IInputMap
{
    #region Properties
    public string Sequence { get; }

    public IState? State { get; set; }

    public Func<IToken>? Token { get; set; }

    public Func<IToken, IToken>? ExpansionMethod { get; set; }

    #endregion

}