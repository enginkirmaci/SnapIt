using System.Windows;

namespace SnapIt.Library.Services
{
	public interface INotifyIconService
	{
		void Initialize();

		void SetApplicationWindow(Window window);

		void Release();
	}
}