using SnapIt.Entities;

namespace SnapIt.Services
{
    public class WindowService : IWindowService
    {
        public SnapWindow CreateSnapWindow()
        {
            var window = new SnapWindow();

            window.CreateGrids();

            return window;
        }
    }
}