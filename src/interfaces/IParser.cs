namespace Interfaces;

public interface IParser
{
    #region Properties
    #endregion

    #region Methods
    public ITree Parse(Queue<IToken> tokens);

    #endregion

}
