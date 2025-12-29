using Interfaces;

namespace Shell.Extensions.ShellState;

public class AppendStdOutState : IState
{
    public AppendStdOutState() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
