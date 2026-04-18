namespace Interfaces;

public interface ITreeNode
{
    #region Properties
    public ITreeNode? Parent { get; set; }

    public ITreeNode? LeftChild { get; set; }

    public ITreeNode? RightChild { get; set; }

    #endregion
    
    #region Methods
    public ITreeNode GetLastChild();

    public void RemoveChild(ITreeNode child);

    #endregion

}
