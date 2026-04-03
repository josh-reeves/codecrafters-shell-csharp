using Interfaces;

namespace Shell.Extensions.ShellInputHandler.Parser;

public class CommandTree : ITree
{
    #region Constructor(s)
    public CommandTree() {}

    #endregion

    #region Properties
    public ITreeNode? Root { get; set; }

    #endregion
    
}
