using Interfaces;

namespace Shell.Commands;

public class ChangeDirectory : ShellCommand
{
    private const string invalidDirMsg = ": No such file or directory";

    public ChangeDirectory(IShellEnvironment environment) : base(environment) {}

    public override void Execute(object[]? args)
    {
        string dir = (args?[0] as string ?? string.Empty).Replace(Environment.HomeChar.ToString(), Environment.HomeDir);

        if (string.IsNullOrEmpty(dir))
        {
            return;

        }

        if (!Directory.Exists(dir))
        {
            Console.WriteLine(dir + invalidDirMsg);

            return;

        }

        Directory.SetCurrentDirectory(dir);

    }

}