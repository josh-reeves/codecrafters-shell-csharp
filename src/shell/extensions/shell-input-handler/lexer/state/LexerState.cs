using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Lexer.State;

public abstract class LexerState : IState
{
    #region Constructor(s)
    public LexerState() {}

    #endregion

    #region Properties
    public IStateController? Controller { get; set; }

    #endregion

    #region Methods
    public virtual void Enter()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        if (controller.TokenMap.ContainsKey(this))
        {
            controller.CurrentToken ??= controller.TokenMap[this].Invoke();
            controller.CurrentToken.Position = controller.Position;

        }

    }

    public virtual void Execute() {} 

    public virtual void Exit()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {            
            controller.AppendToken();
        
        }
        
    }

    #endregion

}

public class LexerDefaultState : LexerState
{
    #region Constructor(s)
    public LexerDefaultState() {}

    #endregion

    #region Methods
    public override void Execute() 
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        for (int i = controller.StateMap.Keys.MaxBy(str => str.Length)?.Length ?? 0; i > 0; i--)
        {            
            string seq = controller.RemainingText.Length >= i ? controller.RemainingText[0..i] : string.Empty;

            if (controller.StateMap.ContainsKey(seq))
            {
                controller.Transition(controller.StateMap[seq]);

                return;

            }

        }

        controller.ConsumeInput();

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(controller.DefaultState);
            
        }
        
    }

    #endregion

}

public class LexerGroupDelimiterState : LexerState
{
    #region Fields
    private char? escape;
    private char terminator;

    #endregion

    #region Constructor(s)
    public LexerGroupDelimiterState(char terminatorChar, char? escapeChar = null)
    {
        terminator = terminatorChar;
        escape = escapeChar;

    }

    #endregion

    #region Methods
    public override void Execute()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        controller.ConsumeInput();

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {            
            controller.Transition(controller.DefaultState);

            return;
        
        }

        if (escape is not null && controller.RemainingText[0] == escape)
        {
            controller.Transition(new LexerEscapeState(this));
            
        }

        if (controller.RemainingText[0] == terminator)
        {
            controller.ConsumeInput();

            controller.Transition(controller.DefaultState);

        }

    }
/*
    private void MissingDelimiterCheck()
    {
        if (Controller is not ILexerStateController controller || !string.IsNullOrWhiteSpace(controller.Lexer.RemainingText))
        {
            return;

        }

        string prompt = "delim> ";

        Console.Write(prompt);
        controller.Lexer.RemainingText += Console.ReadLine();
        
    }
*/

    #endregion

}

public class LexerOperatorState : LexerState
{
    #region Fields
    private string seq;

    #endregion

    #region Constructor(s)
    public LexerOperatorState(string sequence)
    {
        seq = sequence;
        
    }

    #endregion

    #region Methods
    public override void Enter()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        controller.AppendToken();
 
        controller.CurrentToken = controller.TokenMap[this].Invoke();
        controller.CurrentToken.Position = controller.Position;

    }

    public override void Execute()
    {
        if (Controller is not ILexerStateController controller || controller.CurrentToken is null)
        {
            return;

        }

        controller.ConsumeInput();

        if (string.IsNullOrWhiteSpace(controller.RemainingText))
        {
            controller.Transition(controller.DefaultState);

            return;
            
        }

        if (controller.CurrentToken.RawValue == seq)
        {
            controller.Transition(controller.DefaultState);

        }

    }

    #endregion

}

public class LexerSeparatorState : LexerState
{
    #region Constructor(s)
    public LexerSeparatorState() {}

    #endregion

    #region Methods
    public override void Enter()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }
                
        controller.AppendToken();

    }

    public override void Execute()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        controller.ConsumeInput();

        string next = controller.RemainingText[0].ToString();

        if (!controller.StateMap.ContainsKey(next) || controller.StateMap[next] is not LexerSeparatorState)
        {
            controller.Transition(controller.DefaultState);

            return;

        }
 
    }

    #endregion

}

public class LexerEscapeState : LexerState
{
    #region Fields
    private IState? previous;
    #endregion

    #region Constructor(s)
    public LexerEscapeState(IState? previousState = null) =>
        previous = previousState;

    #endregion

    #region Methods
    public override void Execute()
    {
        if (Controller is not ILexerStateController controller)
        {
            return;

        }

        controller.ConsumeInput(2);

        controller.Transition(previous ?? controller.DefaultState);

    }

    #endregion

}