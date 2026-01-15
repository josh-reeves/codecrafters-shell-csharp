using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class CommandTree : ITree
{
    public CommandTree() {}

    public ITreeNode? Root { get; set; }

}
