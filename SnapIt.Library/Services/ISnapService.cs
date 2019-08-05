namespace SnapIt.Library.Services
{
	public interface ISnapService
	{
		event GetStatus StatusChanged;

		void Initialize();

		void Release();
	}

	public delegate void GetStatus(bool isRunning);
}