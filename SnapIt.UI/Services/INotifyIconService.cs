using System.Windows;

namespace SnapIt.UI.Services
{
    public interface INotifyIconService
    {
        void Initialize();

        void SetApplicationWindow(Window window);

        void Release();
    }
}