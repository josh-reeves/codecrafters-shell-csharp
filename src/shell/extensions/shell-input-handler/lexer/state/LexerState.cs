using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public abstract class LexerState : IState
{
    public LexerState() {}

    public IStateController? Controller { get; set; }

    public virtual void Enter() {}

    public virtual void Execute() {} 

    public virtual void Exit() {}

}
