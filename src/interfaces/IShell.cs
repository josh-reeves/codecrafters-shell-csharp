namespace Interfaces;

public interface IShell
{
    public void Run();

    public IList<string>? Args { get; }

}
