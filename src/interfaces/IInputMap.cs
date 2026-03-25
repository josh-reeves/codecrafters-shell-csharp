namespace Interfaces;

public interface IInputMap
{
    #region Properties
    public IState? State { get; }

    public Func<IToken>? Token { get; }

    public Func<string, string>? ExpansionMethod { get; }

    #endregion

}
