using Interfaces;

namespace Shell.State;

public class ShellState : IState
{
    public ShellState() {}

    public IStateController? Controller { get; set; }

    public virtual void Enter() {}

    public virtual void Execute() {}

    public virtual void Exit() {}
    
}
