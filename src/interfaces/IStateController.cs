namespace Interfaces;

public interface IStateController
{
    public IState CurrentState { get; }
    
    public void Transition(IState state);

}
