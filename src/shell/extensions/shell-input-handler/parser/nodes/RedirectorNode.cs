using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class RedirectorNode : ITreeNode
{
    #region Constructors
    public RedirectorNode(IShellToken data, IShellToken fileToken, FileMode fileMode, ITreeNode? parent = null)
    {
        Data = data;
        FileToken = fileToken;
        FileMode = fileMode;
        Parent = parent;
        Children = [];
        
    }

    #endregion

    #region Properties
    public IShellToken Data;

    public IShellToken FileToken { get; }

    public FileMode FileMode { get; set; }

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

    #endregion

}