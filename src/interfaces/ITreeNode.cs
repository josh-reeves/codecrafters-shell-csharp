namespace Interfaces;

public interface ITreeNode
{
    #region Properties
    public ITreeNode? Parent { get; set; }

    public IList<ITreeNode> Children { get; }

    #endregion

}
