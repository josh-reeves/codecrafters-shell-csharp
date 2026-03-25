using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class CommandNode : ITreeNode
{
    public CommandNode(IShellToken data, ITreeNode? parent = null)
    {
        Data = data;
        Parent = parent;
        Children = [];
        
    }

    public IShellToken Data { get; } 

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

}
