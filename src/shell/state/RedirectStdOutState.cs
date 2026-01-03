using Interfaces;

namespace Shell.State;

public class RedirectStdOutState : ShellState
{
    public RedirectStdOutState() {}

    public override void Enter()
    {
        if (Controller is not ShellStateController controller)
        {
            return;
            
        }
        
        controller.Shell.OutputWriter = new StreamWriter(new FileStream(controller.Shell.Filename, FileMode.Create, FileAccess.Write))
        {
            AutoFlush = true

        };

        Console.SetOut(controller.Shell.OutputWriter);

        controller.Shell.IsOutputRedirected = true;

    }

    public override void Execute()
    {
        if (Controller is not ShellStateController controller)
        {
            return;

        }

        controller.Transition(new ShellExecutionState());

    }

}
