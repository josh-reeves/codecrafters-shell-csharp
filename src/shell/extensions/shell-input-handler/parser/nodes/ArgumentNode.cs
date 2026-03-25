using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class ArgumentNode : ITreeNode
{
    public ArgumentNode(IShellToken data, ITreeNode? parent = null)
    {
        Data = data;
        Parent = parent;
        Children = [];
        
    }

    public IShellToken Data { get; }

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

}
