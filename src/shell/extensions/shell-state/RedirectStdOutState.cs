using Interfaces;

namespace Shell.Extensions.ShellState;

public class RedirectStdOutState : IState
{
    private StreamWriter? writer;
    private ShellStateController? controller;

    public RedirectStdOutState() {}

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
        string? file = controller?.Shell?.Args?[1] ?? string.Empty;

        writer = new StreamWriter(file ?? throw new ArgumentNullException());
        writer.AutoFlush = true;

        Console.SetOut(writer);

    }

    public void Execute() {}

    public void Exit()
    {
        if (writer is not null)
            writer.Close();
        
    }

}
