using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class CommandNode : ITreeNode
{
    public CommandNode(IToken data, ITreeNode? parent = null)
    {
        Data = data;
        Parent = parent;
        Children = [];
        
    }

    public IToken Data { get; } 

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

}
