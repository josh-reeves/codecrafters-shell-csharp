using Interfaces;

namespace Shell.State;

public class ShellExecutionState : ShellState
{
    public ShellExecutionState()
    {
        
    }

    public override void Execute()
    {
        if (Controller is not ShellStateController controller)
        {
            return;
            
        }

        IShell shell = controller.Shell;

        if (shell.ShellEnvironment.Commands.Keys.Contains(shell.Parser.TokenizedInput[0].Value))
        {
            shell.ShellEnvironment.Commands[shell.Parser.TokenizedInput[0].Value]?.Execute(shell.Args.ToArray());

        }
        else if (shell.ShellEnvironment.IsExecutable(shell.ShellEnvironment.Search(shell.Command, shell.ShellEnvironment.PathList).ToArray()))
        {
            shell.ShellEnvironment.ExecuteExternal(shell.Command, shell.Args.ToArray());
            
        }
        else
        {
            Console.WriteLine(shell.Command + ": command not found");

        }

        controller.Transition(new ShellInputState());

        return;
        
    }

    public override void Exit()
    {
        if (Controller is not ShellStateController controller)
        {
            return;
            
        }

    }


}
