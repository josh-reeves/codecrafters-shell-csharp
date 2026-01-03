using Interfaces;

namespace Shell.Extensions.Parser.State;

public class ParserEOFState : ParserState
{
    public ParserEOFState() {}
    
    public override void Enter() 
    {
        if (Controller is not ParserStateController controller)
        {
            return;

        }

        /* If the program somehow ended up here prematurely, this will ensure
         *  completion of any calling loops based on the remaining text.*/
        controller.RemainingText = string.Empty; 
        
    }

}
