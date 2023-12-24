using SnapIt.Common.Contracts;

namespace SnapIt.Services.Contracts;

public interface IWindowsService : IInitialize
{
    bool IsExcludedApplication(string Title, bool isKeyboard);

    bool DisableIfFullScreen();
}