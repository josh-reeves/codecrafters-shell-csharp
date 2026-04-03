namespace Interfaces;

public interface IState
{
    #region Properties
    public IStateController? Controller { get; set; }

    #endregion

    #region Methods
    public void Enter();

    public void Execute();

    public void Exit();

    #endregion

}
