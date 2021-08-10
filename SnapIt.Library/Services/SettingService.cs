using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnapIt.Library.Entities;
using Windows.ApplicationModel;
using WpfScreenHelper;

namespace SnapIt.Library.Services
{
    public class SettingService : ISettingService
    {
        private readonly IFileOperationService fileOperationService;

        public Settings Settings { get; private set; }
        public ExcludedApplicationSettings ExcludedApplicationSettings { get; private set; }
        public IList<Layout> Layouts { get; private set; }
        public IList<SnapScreen> SnapScreens { get; private set; }
        public SnapScreen LatestActiveScreen { get; set; }

        public SettingService(
            IFileOperationService fileOperationService)
        {
            this.fileOperationService = fileOperationService;

            Settings = this.fileOperationService.Load<Settings>();
            if (Settings == null)
            {
                Settings = new Settings();
            }

            ExcludedApplicationSettings = this.fileOperationService.Load<ExcludedApplicationSettings>();
            Layouts = this.fileOperationService.GetLayouts();

            ReInitialize();
        }

        public void ReInitialize()
        {
            SnapScreens = GetSnapScreens();

            if (LatestActiveScreen == null)
            {
                LatestActiveScreen = SnapScreens.FirstOrDefault(screen => screen.Base.Primary);
            }
        }

        public void Save()
        {
            Settings.ActiveScreens = SnapScreens.Where(s => s.IsActive).Select(s => s.Base.DeviceName).ToList();

            fileOperationService.Save(Settings);

            foreach (var layout in Layouts.Where(i => i.Status == LayoutStatus.NotSaved))
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
            layout.Status = LayoutStatus.Saved;
            fileOperationService.SaveLayout(layout);
        }

        public void ExportLayout(Layout layout, string layoutPath)
        {
            fileOperationService.ExportLayout(layout, layoutPath);
        }

        public void DeleteLayout(Layout layout)
        {
            Layouts.Remove(layout);
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

                snapScreen.IsActive = Settings.ActiveScreens.Contains(snapScreen.Base.DeviceName);

                snapScreens.Add(snapScreen);
            }

            if (snapScreens.Any(s => !s.IsActive))
            {
                snapScreens.ForEach(s => s.IsActive = true);
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
            catch (Exception ex)
            {
            }
        }
    }
}