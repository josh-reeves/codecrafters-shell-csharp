using Interfaces;

namespace Shell.Extensions.ShellState;

public class AppendStdOut : IState
{
    public AppendStdOut() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
