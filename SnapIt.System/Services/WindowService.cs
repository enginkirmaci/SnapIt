using System.Collections.Generic;
using SnapIt.Controls;
using SnapIt.Entities;

namespace SnapIt.Services
{
    public class WindowService : IWindowService
    {
        private readonly ISettingService settingService;

        private List<SnapWindow> snapWindows;

        public WindowService(
            ISettingService settingService
            )
        {
            this.settingService = settingService;

            snapWindows = new List<SnapWindow>();
        }

        public bool IsVisible
        {
            get => snapWindows.TrueForAll(window => window.IsVisible);
        }

        public void Initialize()
        {
            foreach (var screen in settingService.SnapScreens)
            {
                var window = new SnapWindow(screen);

                window.CreateGrids();

                snapWindows.Add(window);
            }
        }

        public void Release()
        {
            snapWindows.ForEach(window => window.Close());
            snapWindows.Clear();
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