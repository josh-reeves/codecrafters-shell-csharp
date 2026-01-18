using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class RedirectorNode : ITreeNode
{
    #region Constructors
    public RedirectorNode(IToken data, IToken fileToken, FileMode fileMode, ITreeNode? parent = null)
    {
        Data = data;
        FileToken = fileToken;
        FileMode = fileMode;
        Parent = parent;
        Children = [];
        
    }

    public IToken Data;

    public IToken FileToken { get; }

    public FileMode FileMode { get; set; }

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

    #endregion

}