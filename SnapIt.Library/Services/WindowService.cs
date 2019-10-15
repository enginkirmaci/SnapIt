using System.Collections.Generic;
using System.Linq;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public class WindowService : IWindowService
    {
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;
        private readonly List<SnapWindow> snapWindows;

        public WindowService(
            ISettingService settingService,
            IWinApiService winApiService
            )
        {
            this.settingService = settingService;
            this.winApiService = winApiService;
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
                var window = new SnapWindow(winApiService, screen);

                if (!snapWindows.Any(i => i.Screen == screen))
                {
                    window.ApplyLayout();

                    snapWindows.Add(window);
                }

                //break; //TODO test
            }

            snapWindows.ForEach(window =>
            {
                window.Opacity = 0;
                window.Show();
                window.GenerateSnapAreaBoundries();
                window.Hide();
                window.Opacity = 100;
            });
        }

        public void Release()
        {
            snapWindows.ForEach(window =>
            {
                window.Close();
            });
            snapWindows.Clear();
        }

        public void Show()
        {
            snapWindows.ForEach(window =>
            {
                window.Show();
                window.Activate();
            });
        }

        public void Hide()
        {
            snapWindows.ForEach(window => window.Hide());
        }

        public IList<Rectangle> SnapAreaBoundries()
        {
            var boundries = new List<Rectangle>();

            snapWindows.ForEach(window => boundries.AddRange(window.SnapAreaBoundries));

            return boundries;
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