namespace Shell;

class Shell
{
#region Fields
    private const char cmdSep = ' ';
    private const string pathVar = "PATH",
                         invalidCmdMsg = ": command not found",
                         builtinMsg = " is a shell builtin";

    private char pathSep,
                 dirSep;
    private bool shellActive;

    private string[] input;

#endregion
        
#region Constructor(s)
    public Shell()
    {
        shellActive = false;
        pathSep = System.IO.Path.PathSeparator;
        dirSep = System.IO.Path.DirectorySeparatorChar;

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

    public string Path { get => Environment.GetEnvironmentVariable(pathVar) ?? string.Empty;}

    public List<string> PathList { get => Path.Split(pathSep).ToList(); }

#endregion

#region Methods
    public void StartREPL()
    {
        shellActive = true;

        while (shellActive)
        {
            Console.Write("$ ");

            input = Console.ReadLine()?.Split(cmdSep) ?? [ string.Empty ];

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
                Console.Write(cmdSep);
                
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

            return;
            
        }

        foreach(string dir in PathList)
        {
            string file = dir + dirSep + input[1];

            if (FileIsExecutable(file))
            {
                Console.WriteLine(input[1] + " is " + file );

                return;
                
            }

        }

        Console.WriteLine(input[1] + cmdNotFoundMsg);
            
        
    }

    private bool FileIsExecutable(string file)
    {
        if (!File.Exists(file))
        {
            return false;

        }

        if (!OperatingSystem.IsWindows())
        {
            string fileMode = File.GetUnixFileMode(file).ToString().ToLower();

            if (fileMode.Contains("execute"))
            {
                return true;
                
            }
            
        }

        return false;
    
    }


#endregion
    
}