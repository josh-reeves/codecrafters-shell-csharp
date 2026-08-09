using Interfaces;

namespace Shell;

public static class InputMethods
{
    #region Constructor(s)
    static InputMethods() {}

    #endregion

    #region Methods   
    public static string Backspace(string input)
    {
        if (input.Length > 0)
        {
            input = input.Remove(input.Length - 1);
            Console.Write("\b \b");
        
        }

        return input;

    }

    #endregion


}