using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class RedirectorNode : ITreeNode
{
    public RedirectorNode(IToken fileToken, FileMode fileMode, ITreeNode? parent = null)
    {
        FileToken = fileToken;
        FileMode = fileMode;
        Parent = parent;
        Children = [];
        
    }

    public IToken FileToken { get; }

    public FileMode FileMode { get; set; }

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

}
