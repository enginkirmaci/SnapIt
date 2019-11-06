using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using Windows.ApplicationModel;

namespace SnapIt.Library.Services
{
    public class SettingService : ISettingService
    {
        private readonly IFileOperationService fileOperationService;

        public Settings Settings { get; private set; }
        public ExcludedApplicationSettings ExcludedApplicationSettings { get; private set; }
        public IList<Layout> Layouts { get; private set; }
        public IList<SnapScreen> SnapScreens { get; private set; }

        public SettingService(
            IFileOperationService fileOperationService)
        {
            this.fileOperationService = fileOperationService;

            Settings = this.fileOperationService.Load<Settings>();
            ExcludedApplicationSettings = this.fileOperationService.Load<ExcludedApplicationSettings>();
            Layouts = this.fileOperationService.GetLayouts();
            SnapScreens = GetSnapScreens();
        }

        public void ReInitialize()
        {
            SnapScreens = GetSnapScreens();
        }

        public void Save()
        {
            fileOperationService.Save(Settings);

            foreach (var layout in Layouts.Where(i => !i.IsSaved))
            {
                SaveLayout(layout);
            }
        }

        public void SaveExcludedApps(List<ExcludedApplication> excludedApplications)
        {
            ExcludedApplicationSettings.Applications = excludedApplications;

            fileOperationService.Save(ExcludedApplicationSettings);
        }

        public void SaveLayout(Layout layout)
        {
            layout.IsSaved = true;
            fileOperationService.SaveLayout(layout);
        }

        public void ExportLayout(Layout layout, string layoutPath)
        {
            fileOperationService.ExportLayout(layout, layoutPath);
        }

        public void DeleteLayout(Layout layout)
        {
            fileOperationService.DeleteLayout(layout);
        }

        public Layout ImportLayout(string layoutPath)
        {
            return fileOperationService.ImportLayout(layoutPath);
        }

        public void LinkScreenLayout(SnapScreen snapScreen, Layout layout)
        {
            SnapScreens.First(screen => screen.Base.DeviceName == snapScreen.Base.DeviceName).Layout = layout;

            if (Settings.ScreensLayouts.ContainsKey(snapScreen.Base.DeviceName))
            {
                Settings.ScreensLayouts[snapScreen.Base.DeviceName] = layout.Guid.ToString();
            }
            else
            {
                Settings.ScreensLayouts.Add(snapScreen.Base.DeviceName, layout.Guid.ToString());
            }
        }

        private IList<SnapScreen> GetSnapScreens()
        {
            var snapScreens = new List<SnapScreen>();

            foreach (var screen in Screen.AllScreens)
            {
                var snapScreen = new SnapScreen(screen);
                var layoutGuid = Settings.ScreensLayouts.ContainsKey(snapScreen.Base.DeviceName)
                    ? Settings.ScreensLayouts[snapScreen.Base.DeviceName] : string.Empty;

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

        public async Task<bool> GetStartupTaskStatusAsync()
        {
            try
            {
                var startupTask = await StartupTask.GetAsync("SnapItStartupTask"); // Pass the task ID you specified in the appxmanifest file
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                    case StartupTaskState.DisabledByUser:
                    case StartupTaskState.DisabledByPolicy:
                        return false;

                    case StartupTaskState.Enabled:
                    case StartupTaskState.EnabledByPolicy:
                    default:
                        return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SetStartupTaskStatusAsync(bool isActive)
        {
            try
            {
                var startupTask = await StartupTask.GetAsync("SnapItStartupTask");
                if (isActive)
                {
                    await startupTask.RequestEnableAsync();
                }
                else
                {
                    startupTask.Disable();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}