using Interfaces;

namespace Shell.State;

public class ShellStateController : IStateController
{
    private IState defaultState,
                   currentState;

    public ShellStateController(IShell shell, IState initialState)
    {
        Shell = shell;
        
        currentState = defaultState = initialState;       
        currentState.Controller = this;
        currentState.Enter(); 

    }

    public IShell Shell { get; private set;}

    public IState CurrentState
    {
        get => currentState;

        set
        {
            currentState = value;
            currentState.Controller = this;

        }
        
    }

    public void Transition(IState state)
    {

        try
        {
            CurrentState.Exit();

            CurrentState = state;

            CurrentState.Enter();

        }
        catch
        {
            CurrentState = defaultState;

        }

    }


}
