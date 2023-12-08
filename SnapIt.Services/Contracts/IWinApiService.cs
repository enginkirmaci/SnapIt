using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Services.Contracts;

public interface IWinApiService : IInitialize
{
    IDictionary<nint, string> GetOpenWindows();

    IEnumerable<string> GetOpenWindowsNames();

    bool IsFullscreen(ActiveWindow activeWindow);

    bool IsAllowedWindowStyle(ActiveWindow activeWindow);

    void MoveWindow(ActiveWindow activeWindow, Rectangle newRect);

    bool MoveWindow(ActiveWindow activeWindow, int X, int Y, int width, int height);

    void SendMessage(ActiveWindow activeWindow);

    void GetWindowMargin(ActiveWindow activeWindow, out Rectangle withMargin);

    ActiveWindow GetActiveWindow();

    string GetCurrentDesktopWallpaper();
}