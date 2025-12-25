using Interfaces;

namespace Shell.Commands;

public class PrintWorkingDirectory : ShellCommand
{
    public PrintWorkingDirectory(IShellEnvironment environment) : base(environment) {}
    
    public override void Execute(object[]? args)
        => Console.WriteLine(Directory.GetCurrentDirectory());

}