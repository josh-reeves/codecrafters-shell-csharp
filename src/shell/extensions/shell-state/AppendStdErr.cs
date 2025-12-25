using Interfaces;

namespace Shell.Extensions.ShellState;

public class AppendStdErr : IState
{
    public AppendStdErr() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
