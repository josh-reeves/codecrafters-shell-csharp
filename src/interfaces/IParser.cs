namespace Interfaces;

public interface IParser
{
    #region Methods
    public ITree Parse(Queue<IToken> tokens);

    #endregion

}
