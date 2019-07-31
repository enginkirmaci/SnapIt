using SnapIt.Entities;

namespace SnapIt.Services
{
	public interface IWindowService
	{
		bool IsVisible { get; }

		void Initialize();

		void Release();

		void Show();

		void Hide();

		Rectangle SelectElementWithPoint(int x, int y);

		event EscKeyPressedDelegate EscKeyPressed;
	}

	public delegate void EscKeyPressedDelegate();
}