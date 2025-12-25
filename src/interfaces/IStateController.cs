namespace Interfaces;

public interface IStateController
{
    public IState CurrentState { get; }
    
    public Dictionary<IState, HashSet<IState>> Transitions { get; }

    public void Transition(IState state);

}
