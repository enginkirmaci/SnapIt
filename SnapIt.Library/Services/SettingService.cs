using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SnapIt.Library.Configuration;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using Windows.ApplicationModel;

namespace SnapIt.Library.Services
{
    public class SettingService : ISettingService
    {
        private readonly IConfigService configService;

        public Config Config { get; private set; }
        public IList<Layout> Layouts { get; private set; }
        public IList<SnapScreen> SnapScreens { get; private set; }
        public bool IsStartupTaskActive { get => GetStartupTaskStatus(); set => SetStartupTaskStatus(value); }

        public SettingService(
            IConfigService configService)
        {
            this.configService = configService;

            Config = configService.Load<Config>();
            Layouts = configService.GetLayouts();
            SnapScreens = GetSnapScreens();
        }

        public void Save()
        {
            configService.Save(Config);

            foreach (var layout in Layouts.Where(i => !i.IsSaved))
            {
                SaveLayout(layout);
            }
        }

        public void SaveLayout(Layout layout)
        {
            layout.IsSaved = true;
            configService.SaveLayout(layout);
        }

        public void ExportLayout(Layout layout, string layoutPath)
        {
            configService.ExportLayout(layout, layoutPath);
        }

        public Layout ImportLayout(string layoutPath)
        {
            return configService.ImportLayout(layoutPath);
        }

        public void LinkScreenLayout(SnapScreen snapScreen, Layout layout)
        {
            SnapScreens.First(screen => screen.Base.DeviceName == snapScreen.Base.DeviceName).Layout = layout;

            if (Config.ScreensLayouts.ContainsKey(snapScreen.Base.DeviceName))
            {
                Config.ScreensLayouts[snapScreen.Base.DeviceName] = layout.Guid.ToString();
            }
            else
            {
                Config.ScreensLayouts.Add(snapScreen.Base.DeviceName, layout.Guid.ToString());
            }
        }

        private IList<SnapScreen> GetSnapScreens()
        {
            var snapScreens = new List<SnapScreen>();

            foreach (var screen in Screen.AllScreens)
            {
                var snapScreen = new SnapScreen(screen);
                var layoutGuid = Config.ScreensLayouts.ContainsKey(snapScreen.Base.DeviceName)
                    ? Config.ScreensLayouts[snapScreen.Base.DeviceName] : string.Empty;

                if (!string.IsNullOrWhiteSpace(layoutGuid))
                {
                    snapScreen.Layout = Layouts.FirstOrDefault(layout => layout.Guid.ToString() == layoutGuid);
                }
                else
                {
                    snapScreen.Layout = Layouts.FirstOrDefault();
                }

                snapScreens.Add(snapScreen);
            }

            return snapScreens;
        }

        private bool GetStartupTaskStatus()
        {
            var startupTask = StartupTask.GetAsync("SnapItStartupTask").GetResults(); // Pass the task ID you specified in the appxmanifest file
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                case StartupTaskState.DisabledByUser:
                case StartupTaskState.DisabledByPolicy:
                    return false;
            }

            return true;
        }

        private void SetStartupTaskStatus(bool isActive)
        {
            var startupTask = StartupTask.GetAsync("SnapItStartupTask").GetResults();
            if (isActive)
            {
                startupTask.RequestEnableAsync().GetResults();
            }
            else
            {
                startupTask.Disable();
            }
        }
    }
}