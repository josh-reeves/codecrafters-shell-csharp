using System.Diagnostics;
using System.IO.Enumeration;

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
            {"type", () => Type() },
            {"pwd", () => Console.WriteLine(Directory.GetCurrentDirectory()) },
            {"cd", () => Echo() }

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

            if (Commands.Keys.Contains(input[0]))
            {
                 Commands[input[0]]?.Invoke();
               
            }
            else if (IsExecutable(input[0]))
            {
                Process process = new();

                process.StartInfo.FileName = input[0];

                for (int i = 1; i < input.Length; i ++ )
                {
                    process.StartInfo.ArgumentList.Add(input[i]);

                }

                process.Start();

                process.WaitForExit();

            }
            else
            {
                Console.WriteLine(input[0] + invalidCmdMsg);

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

        if (IsExecutable(input[1], out string path))
        {
            Console.WriteLine(input[1] + " is " + path);

            return;
            
        }

        Console.WriteLine(input[1] + cmdNotFoundMsg);   
        
    }

    private bool IsExecutable(string fileName)
    {
        foreach(string dir in PathList)
        {
            string file = dir + dirSep + fileName;

            if (!File.Exists(file))
            {
                continue;

            }

            if (!OperatingSystem.IsWindows())
            {
                string fileMode = File.GetUnixFileMode(file).ToString().ToLower();

                if (fileMode.Contains("execute"))
                {
                    return true;
                    
                }
                
            }

        }

        return false;
    
    }

    private bool IsExecutable(string fileName, out string path)
    {
        foreach(string dir in PathList)
        {
            string file = dir + dirSep + fileName;

            if (!File.Exists(file))
            {
                continue;

            }

            if (!OperatingSystem.IsWindows())
            {
                string fileMode = File.GetUnixFileMode(file).ToString().ToLower();

                if (fileMode.Contains("execute"))
                {
                    path = file;

                    return true;
                    
                }
                
            }

        }

        path = string.Empty;

        return false;
    
    }

    private IEnumerable<string>? Search(string file, IEnumerable<string> directories)
    {
        List<string>? results = null;

        foreach (string dir in directories)
        {
            string path = dir + dirSep + file;

            if (File.Exists(path))
            {
                results ??= new();

                results.Add(path);

            }
        
        }
        
        return results;

    }

#endregion
    
}