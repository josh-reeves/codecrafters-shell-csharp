using Interfaces;

namespace Shell.State;

public class AppendStdErrState : IState
{
    public AppendStdErrState() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
