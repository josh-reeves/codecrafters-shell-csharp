using Interfaces;

namespace Shell.Extensions.Parser.State;

public abstract class ParserState : IState
{
    public ParserState() {}

    public IStateController? Controller { get; set; }

    public virtual void Enter() {}

    public virtual void Execute() {} 

    public virtual void Exit() {}

}
