namespace Interfaces;

public interface IState
{
    public IStateController? Controller { get; set; }

    public void Enter();

    public void Execute();

    public void Exit();

}
