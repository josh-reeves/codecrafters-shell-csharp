using Interfaces;

namespace Shell.Extensions.ShellState;

public class RedirectStdErr : IState
{
    public RedirectStdErr() {}

    public IStateController? Controller { get; set; }

    public void Enter() {}

    public void Execute() {}

    public void Exit() {}

}
