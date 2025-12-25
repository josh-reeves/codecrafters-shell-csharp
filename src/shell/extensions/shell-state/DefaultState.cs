using Interfaces;

namespace Shell.Extensions.ShellState;

public class DefaultState : IState
{
    private ShellStateController? controller;

    public DefaultState() {}

    public IStateController? Controller
    {
        get => controller;

        set
        {
            if (value is not ShellStateController shellController)
            {
                return;

            }

            controller = shellController;

        }

    }

    public void Enter()
    {
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()));

    }

    public void Execute() {}

    public void Exit() {}

}
