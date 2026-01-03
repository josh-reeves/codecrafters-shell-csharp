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

        if (shell.Commands.Keys.Contains(shell.Parser.TokenizedInput[0].Value))
        {
            shell.Commands[shell.Parser.TokenizedInput[0].Value]?.Execute(shell.Args.ToArray());

        }
        else if (shell.IsExecutable(shell.Search(shell.Command, shell.PathList).ToArray()))
        {
            shell.ExecuteExternal(shell.Command, shell.Args.ToArray());
            
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
