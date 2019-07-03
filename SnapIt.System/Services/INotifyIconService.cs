using System.Windows;

namespace SnapIt.Services
{
    public interface INotifyIconService
    {
        void Initialize();

        void SetApplicationWindow(Window window);

        void Release();
    }
}