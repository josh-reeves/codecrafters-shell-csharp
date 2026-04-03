using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class ArgumentNode : ITreeNode
{
    #region Constructor(s)
    public ArgumentNode(IShellToken data, ITreeNode? parent = null)
    {
        Data = data;
        Parent = parent;
        Children = [];
        
    }

    #endregion

    #region Properties
    public IShellToken Data { get; }

    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

    #endregion
    
}
