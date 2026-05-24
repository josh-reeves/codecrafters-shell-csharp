using System;
using System.Security.Cryptography.X509Certificates;
using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser.Nodes;

public class ShellNode : IShellNode
{
    #region Fields
    private ITreeNode? leftChild,
                       rightChild;

    #endregion

    #region Constructor(s)
    public ShellNode(NodeType nodeType, IShellToken data, ITreeNode? parent = null)
    {
        Data = data;
        Parent = parent;
        NodeType = nodeType;
        
    }

    #endregion

    #region Properties
    public IShellToken Data { get; } 

    public ITreeNode? Parent { get; set; }

    /// <summary>
    /// The node's leftmost child. Note that if a node without a parent is
    ///  assigned this this property, that node's parent will be updated
    ///  automatically.
    /// </summary>
    public ITreeNode? LeftChild
    {
        get => leftChild;

        set
        {
            leftChild = value;

            if (leftChild is not null && leftChild.Parent is null)
            {
                leftChild.Parent = this;


            }

        }
    }

    /// <summary>
    /// The node's rightmost child. Note that if a node without a parent is
    ///  assigned this this property, that node's parent will be updated
    ///  automatically.
    /// </summary>
    public ITreeNode? RightChild
    {
        get => rightChild;

        set
        {
            rightChild = value;

            if (rightChild is not null && rightChild.Parent is null)
            {
                rightChild.Parent = this;

            }

        }

    }

    public NodeType NodeType { get; }

    #endregion

    #region Methods
    public ITreeNode GetLastChild()
    {
        ITreeNode? iterator,
                   result = iterator = this;

        while (iterator is not null)
        {
            result = iterator;

            if (iterator.RightChild is not null)
            {
                iterator = iterator.RightChild;

                continue;

            }

            iterator = iterator.LeftChild;
            
        }

        return result;

    }

    public void RemoveChild(ITreeNode child)
    {
        child.Parent = null;

        if (LeftChild == child)
        {
            LeftChild = null;

        }

        if (RightChild == child)
        {
            RightChild = null;
            
        }
        
    }
    
    #endregion

}
