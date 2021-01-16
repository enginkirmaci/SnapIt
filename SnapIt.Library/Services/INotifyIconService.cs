using System.Windows;
using System.Windows.Forms;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface INotifyIconService
    {
        event SetViewEvent SetView;

        void Initialize();

        void SetApplicationWindow(Window window);

        void ShowNotification(string title, string message, int timeout = 1000, ToolTipIcon tipIcon = ToolTipIcon.None);

        void Release();
    }

    public delegate void SetViewEvent(ViewType viewType);
}