using Interfaces;

namespace Shell.Extensions.ShellState;

public class ShellStateController : IStateController
{
    private IState defaultState;

    public ShellStateController(IShell shell, IState initialState)
    {
        Shell = shell;
        CurrentState = defaultState = initialState;
        Transitions = new();
        
        if (initialState.Controller is null)
            initialState.Controller = this;

    }

    internal IShell Shell { get; private set;}

    public IState CurrentState { get; private set;}

    public Dictionary<IState, HashSet<IState>> Transitions { get; private set;}

    public void Transition(IState state)
    {
        try
        {
            if (!Transitions[CurrentState].TryGetValue(state, out IState? newState))
            {
                return;

            }

            CurrentState.Exit();

            CurrentState = newState;

            CurrentState.Enter();

            CurrentState.Execute();
            
        }
        catch
        {
            CurrentState = defaultState;
            
        }
        
    }

}
