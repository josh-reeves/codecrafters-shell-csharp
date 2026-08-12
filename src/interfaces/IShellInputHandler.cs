namespace Interfaces;

public interface IShellInputHandler : IInputHandler
{
    #region Properties
    IShellReader Reader { get; set; }

    #endregion

}