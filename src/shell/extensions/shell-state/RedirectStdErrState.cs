using Interfaces;

namespace Shell.Extensions.ShellState;

public class RedirectStdErrState : IState
{
    public RedirectStdErrState() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
