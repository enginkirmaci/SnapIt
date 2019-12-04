using System.Windows;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface INotifyIconService
    {
        event SetViewEvent SetView;

        void Initialize();

        void SetApplicationWindow(Window window);

        void Release();
    }

    public delegate void SetViewEvent(ViewType viewType);
}