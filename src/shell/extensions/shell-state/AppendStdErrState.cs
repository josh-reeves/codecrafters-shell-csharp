using Interfaces;

namespace Shell.Extensions.ShellState;

public class AppendStdErrState : IState
{
    public AppendStdErrState() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
