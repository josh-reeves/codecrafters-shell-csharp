using Interfaces;

namespace Shell.Extensions.Parser;

public abstract class ParserState : IState
{
    private ParserStateController? controller;

    public ParserState() {}

    public ParserState(IParserStateController stateController)
    {
        controller = (ParserStateController)stateController;

    }

    public IStateController? Controller 
    { 
        get => controller;
        
        set
        {
            if (value is not ParserStateController)
            {
                return;

            }

            controller = value as ParserStateController;

        }
        
    }

    public virtual void Enter() {}

    public virtual void Execute() {} 

    public virtual void Exit() {}


}
