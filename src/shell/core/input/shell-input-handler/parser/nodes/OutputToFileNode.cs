using Interfaces;

namespace Shell.Core.Input.ShellInputHandler.Parser.Nodes;

public class OutputToFileNode : ShellNode, IOutputToFileNode
{
    #region Constructors
    public OutputToFileNode(IShellToken data, IShellToken fileToken, FileMode fileMode, ITreeNode? parent = null) : base (NodeType.OutputRedirection, data, parent)
    {
        FileToken = fileToken;
        FileMode = fileMode;
        
    }

    #endregion

    #region Properties
    public IShellToken FileToken { get; }

    public FileMode FileMode { get; set; }

    #endregion

}