namespace Interfaces;

public enum NodeType
{
    Command,
    Argument,
    OutputRedirection
    
}

public interface IShellNode : ITreeNode
{
    #region Properties
    public IShellToken Data { get; }

    public NodeType NodeType { get; }

    #endregion

}
