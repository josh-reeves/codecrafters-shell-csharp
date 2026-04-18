namespace Interfaces;

public interface IOutputToFileNode : IShellNode
{
    #region Properties
    public IShellToken FileToken { get; }

    public FileMode FileMode { get; set; }

    #endregion

}
