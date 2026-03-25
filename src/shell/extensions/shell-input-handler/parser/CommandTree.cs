using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser;

public class CommandTree : ITree
{
    public CommandTree() {}

    public ITreeNode? Root { get; set; }

}
