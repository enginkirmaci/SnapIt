using SnapScreen.Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using WpfScreenHelper;

namespace SnapScreen.Library.Services
{
    public class SettingService : ISettingService
    {
        private readonly IFileOperationService fileOperationService;

        public Settings Settings { get; private set; }
        public ExcludedApplicationSettings ExcludedApplicationSettings { get; private set; }
        public StandaloneLicense StandaloneLicense { get; private set; }
        public IList<Layout> Layouts { get; private set; }
        public IList<Entities.SnapScreen> SnapScreens { get; private set; }
        public Entities.SnapScreen LatestActiveScreen { get; set; }

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

#if STANDALONE
            StandaloneLicense = this.fileOperationService.Load<StandaloneLicense>();

            if (StandaloneLicense == null)
            {
                StandaloneLicense = new StandaloneLicense();
            }
#endif

            ReInitialize();
        }

        public void ReInitialize()
        {
            SnapScreens = GetSnapScreens();

            if (LatestActiveScreen == null)
            {
                LatestActiveScreen = SnapScreens.FirstOrDefault(screen => screen.IsPrimary);
            }
        }

        public void Save()
        {
            foreach (var screen in SnapScreens)
            {
                if (screen.IsActive && Settings.DeactivedScreens.Contains(screen.DeviceName))
                {
                    Settings.DeactivedScreens.Remove(screen.DeviceName);
                }
                else if (!screen.IsActive && !Settings.DeactivedScreens.Contains(screen.DeviceName))
                {
                    Settings.DeactivedScreens.Add(screen.DeviceName);
                }
            }

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

        public void SaveStandaloneLicense(StandaloneLicense standaloneLicense)
        {
            StandaloneLicense = standaloneLicense;
            fileOperationService.Save(StandaloneLicense);
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

        public void LinkScreenLayout(Entities.SnapScreen snapScreen, Layout layout)
        {
            SnapScreens.First(screen => screen.DeviceName == snapScreen.DeviceName).Layout = layout;

            if (Settings.ScreensLayouts.ContainsKey(snapScreen.DeviceName))
            {
                Settings.ScreensLayouts[snapScreen.DeviceName] = layout.Guid.ToString();
            }
            else
            {
                Settings.ScreensLayouts.Add(snapScreen.DeviceName, layout.Guid.ToString());
            }
        }

        private IList<Entities.SnapScreen> GetSnapScreens()
        {
            var snapScreens = new List<Entities.SnapScreen>();

            var displays = WindowsDisplayAPI.Display.GetDisplays();

            foreach (var screen in Screen.AllScreens)
            {
                var display = displays.FirstOrDefault(display => display.DisplayName == screen.DeviceName);
                var snapScreen = new Entities.SnapScreen(screen, display?.DevicePath);
                var layoutGuid = Settings.ScreensLayouts.ContainsKey(snapScreen.DeviceName)
                    ? Settings.ScreensLayouts[snapScreen.DeviceName] : string.Empty;

                if (string.IsNullOrWhiteSpace(layoutGuid)) //fallback for older version
                {
                    layoutGuid = Settings.ScreensLayouts.ContainsKey(snapScreen.DeviceName)
                    ? Settings.ScreensLayouts[snapScreen.DeviceName] : string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(layoutGuid))
                {
                    snapScreen.Layout = Layouts.FirstOrDefault(layout => layout.Guid.ToString() == layoutGuid);
                }
                else
                {
                    snapScreen.Layout = Layouts.FirstOrDefault();
                }

                snapScreen.IsActive = !Settings.DeactivedScreens.Contains(snapScreen.DeviceName);

                snapScreens.Add(snapScreen);
            }

            //if (snapScreens.Any(s => !s.IsActive))
            //{
            //    snapScreens.ForEach(s => s.IsActive = true);
            //}

            return snapScreens;
        }

        public async Task<bool> GetStartupTaskStatusAsync()
        {
#if !STANDALONE
            try
            {
                var startupTask = await StartupTask.GetAsync("SnapScreenStartupTask"); // Pass the task ID you specified in the appxmanifest file
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
            catch (Exception ex)
            {
                return false;
            }
#endif
#if STANDALONE
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                return key.GetValue(Constants.AppRegistryKey) != null;
            }
#endif
        }

        public async Task SetStartupTaskStatusAsync(bool isActive)
        {
#if !STANDALONE
            try
            {
                var startupTask = await StartupTask.GetAsync("SnapScreenStartupTask");
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
#endif
#if STANDALONE
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (isActive)
                {
                    key.SetValue(Constants.AppRegistryKey, "\"" + System.Windows.Forms.Application.ExecutablePath + "\"");
                }
                else
                {
                    key.DeleteValue(Constants.AppRegistryKey, false);
                }
            }
#endif
        }
    }
}