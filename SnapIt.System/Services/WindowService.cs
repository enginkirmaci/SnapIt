using System.Collections.Generic;
using System.Windows.Forms;
using SnapIt.Entities;

namespace SnapIt.Services
{
    public class WindowService : IWindowService
    {
        private List<SnapWindow> snapWindows;

        public WindowService()
        {
            snapWindows = new List<SnapWindow>();
        }

        public bool IsVisible
        {
            get => snapWindows.TrueForAll(window => window.IsVisible);
        }

        public void Initialize()
        {
            foreach (var screen in Screen.AllScreens)
            {
                var window = new SnapWindow(screen);

                window.CreateGrids();

                snapWindows.Add(window);
            }
        }

        public void Release()
        {
            snapWindows.ForEach(window => window.Close());
        }

        public void Show()
        {
            snapWindows.ForEach(window => window.Show());
        }

        public void Hide()
        {
            snapWindows.ForEach(window => window.Hide());
        }

        public Rectangle SelectElementWithPoint(int x, int y)
        {
            var result = new Rectangle();

            foreach (var window in snapWindows)
            {
                var selectedArea = window.SelectElementWithPoint(x, y);
                if (!selectedArea.Equals(Rectangle.Empty))
                {
                    result = selectedArea;
                    break;
                }
            }

            return result;
        }
    }
}