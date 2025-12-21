using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Shell;

class Shell
{
#region Fields
    private const char sep = ' ';
    private const string invalidCmdMsg = ": command not found",
                         builtinMsg = " is a shell builtin";

    private bool shellActive;

    private string[] input;

#endregion
        
#region Constructor(s)
    public Shell()
    {
        shellActive = false;

        input = [];

        Commands = new()
        {
            {"exit", () => shellActive = false },
            {"echo", () => Echo() },
            {"type", () => Type() }

        };
        
    }

#endregion

#region Properties
    public Dictionary<string, Action> Commands { get; private set; }

#endregion

#region Methods
    public void StartREPL()
    {
        shellActive = true;

        while (shellActive)
        {
            Console.Write("$ ");

            input = Console.ReadLine()?.Split(sep) ?? [ string.Empty ];

            if (!Commands.Keys.Contains(input[0]))
            {
                Console.WriteLine(input[0] + invalidCmdMsg);
                
            }
            else
            {
                Commands[input[0]]?.Invoke();

            }

        }
        
    }

    private void Echo()
    {
        for (int i = 1; i < input .Length; i++)
        {
            Console.Write(input[i]);

            if (i == input.Length - 1)
            {
                Console.Write("\r\n");
            }
            else
            {
                Console.Write(sep);
                
            }
            
        }         
        
    }

    private void Type()
    {
        const string cmdNotFoundMsg = ": not found";

        if (input.Length <= 1)
        {
            Console.WriteLine(cmdNotFoundMsg);

            return;

        }

        if (Commands.Keys.Contains(input[1]))
        {
            Console.WriteLine(input[1] + builtinMsg);
            
        }
        else
        {
            Console.WriteLine(input[1] + cmdNotFoundMsg);
            
        }
        
    }

#endregion
    
}