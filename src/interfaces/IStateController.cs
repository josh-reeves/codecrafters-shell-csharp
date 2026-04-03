namespace Interfaces;

public interface IStateController
{
    #region Properties
    public IState CurrentState { get; }
    
    #endregion

    #region Methods
    public void Transition(IState state);

    #endregion
    
}
