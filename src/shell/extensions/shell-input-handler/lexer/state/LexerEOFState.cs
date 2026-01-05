namespace Shell.Extensions.Lexer.State;

public class LexerEOFState : LexerState
{
    public LexerEOFState() {}
    
    public override void Enter() 
    {
        if (Controller is not LexerStateController controller)
        {
            return;

        }

        /* If the program somehow ended up here prematurely, this will ensure
         *  completion of any calling loops based on the remaining text.*/
        controller.Lexer.RemainingText = string.Empty; 
        
    }

}
