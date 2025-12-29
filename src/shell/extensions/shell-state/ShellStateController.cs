using Interfaces;

namespace Shell.Extensions.ShellState;

public class ShellStateController : IStateController
{
    private IState defaultState;

    public ShellStateController(IShell shell, IState initialState)
    {
        Shell = shell;
        CurrentState = defaultState = initialState;
        
        if (initialState.Controller is null)
            initialState.Controller = this;

    }

    internal IShell Shell { get; private set;}

    public IState CurrentState { get; private set;}

    public void Transition(IState state)
    {
        try
        {
            CurrentState.Exit();

            CurrentState = state;

            CurrentState.Enter();

            CurrentState.Execute();
            
        }
        catch
        {
            CurrentState = defaultState;
            
        }
        
    }

}
