using SnapIt.Common.Entities;
using SnapIt.Services.Contracts;
using WpfScreenHelper;

namespace SnapIt.Services;

public class SettingService : ISettingService
{
    private readonly IFileOperationService fileOperationService;

    public bool IsInitialized { get; private set; }
    public Settings Settings { get; private set; }
    public ExcludedApplicationSettings ExcludedApplicationSettings { get; private set; }
    public ApplicationGroupSettings ApplicationGroupSettings { get; private set; }
    public StandaloneLicense StandaloneLicense { get; private set; }
    public IList<Layout> Layouts { get; private set; }
    public IList<SnapScreen> SnapScreens { get; private set; }
    public SnapScreen LatestActiveScreen { get; set; }
    public SnapScreen SelectedSnapScreen { get; set; }

    public SettingService(
        IFileOperationService fileOperationService)
    {
        this.fileOperationService = fileOperationService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        Settings = await fileOperationService.Load<Settings>();

        if (Settings == null)
        {
            Settings = new Settings();
        }

        //Settings.Theme = new SnapAreaTheme(
        //    Settings.HighlightColor,
        //    Settings.OverlayColor,
        //    Settings.BorderColor,
        //    Settings.BorderThickness,
        //    Settings.Opacity);

        ExcludedApplicationSettings = await fileOperationService.Load<ExcludedApplicationSettings>();
        ExcludedApplicationSettings.Applications = ExcludedApplicationSettings.Applications.Where(i => i != null).ToList();

        if (!ExcludedApplicationSettings.Applications.Any(e => e.Keyword == "Program Manager"))
        {
            ExcludedApplicationSettings.Applications.Add(new ExcludedApplication
            {
                Keyword = "Program Manager",
                MatchRule = MatchRule.Contains,
                Mouse = true,
                Keyboard = true
            });
        }
        ApplicationGroupSettings = await fileOperationService.Load<ApplicationGroupSettings>();

        Layouts = fileOperationService.GetLayouts();

        ReInitialize();

        IsInitialized = true;
    }

    public void ReInitialize()
    {
        SnapScreens = GetSnapScreens();

        if (LatestActiveScreen == null || SelectedSnapScreen == null)
        {
            LatestActiveScreen = SelectedSnapScreen = SnapScreens.FirstOrDefault(screen => screen.IsPrimary);
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

        //Settings.HighlightColor = Settings.Theme.HighlightColor.Convert();
        //Settings.OverlayColor = Settings.Theme.OverlayColor.Convert();
        //Settings.BorderColor = Settings.Theme.BorderColor.Convert();
        //Settings.BorderThickness = Settings.Theme.BorderThickness;
        //Settings.Opacity = Settings.Theme.Opacity;

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

    public void LinkScreenLayout(SnapScreen snapScreen, Layout layout)
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

    public void LinkScreenApplicationGroups(SnapScreen snapScreen, List<ApplicationGroup> applicationGroups)
    {
        SnapScreens.First(screen => screen.DeviceName == snapScreen.DeviceName).ApplicationGroups = applicationGroups;

        if (ApplicationGroupSettings.ScreensApplicationGroups == null)
        {
            ApplicationGroupSettings.ScreensApplicationGroups = [];
        }

        if (ApplicationGroupSettings.ScreensApplicationGroups.ContainsKey(snapScreen.DeviceName))
        {
            ApplicationGroupSettings.ScreensApplicationGroups[snapScreen.DeviceName] = applicationGroups;
        }
        else
        {
            ApplicationGroupSettings.ScreensApplicationGroups.Add(snapScreen.DeviceName, applicationGroups);
        }

        SaveApplicationGroupSettings();
    }

    public async Task<bool> GetStartupTaskStatusAsync()
    {
        //#if !STANDALONE
        //        try
        //        {
        //            var startupTask = await StartupTask.GetAsync("SnapItStartupTask"); // Pass the task ID you specified in the appxmanifest file
        //            switch (startupTask.State)
        //            {
        //                case StartupTaskState.Disabled:
        //                case StartupTaskState.DisabledByUser:
        //                case StartupTaskState.DisabledByPolicy:
        //                    return false;

        //                case StartupTaskState.Enabled:
        //                case StartupTaskState.EnabledByPolicy:
        //                default:
        //                    return true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //        }
        //#endif
        //#if STANDALONE
        using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
        {
            return key.GetValue(Constants.AppRegistryKey) != null;
        }
        //#endif
    }

    public async Task SetStartupTaskStatusAsync(bool isActive)
    {
        //#if !STANDALONE
        //        try
        //        {
        //            var startupTask = await StartupTask.GetAsync("SnapItStartupTask");
        //            if (isActive)
        //            {
        //                await startupTask.RequestEnableAsync();
        //            }
        //            else
        //            {
        //                startupTask.Disable();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //#endif
        //#if STANDALONE
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
        //#endif
    }

    public void Dispose()
    {
        IsInitialized = false;
    }

    private IList<SnapScreen> GetSnapScreens()
    {
        var snapScreens = new List<SnapScreen>();

        var displays = WindowsDisplayAPI.Display.GetDisplays();

        foreach (var screen in Screen.AllScreens)
        {
            var display = displays.FirstOrDefault(display => display.DisplayName == screen.DeviceName);
            var snapScreen = new SnapScreen(screen, display?.DevicePath);
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

            snapScreen.ApplicationGroups = ApplicationGroupSettings.ScreensApplicationGroups != null ?
                ApplicationGroupSettings.ScreensApplicationGroups.ContainsKey(snapScreen.DeviceName)
                ? ApplicationGroupSettings.ScreensApplicationGroups[snapScreen.DeviceName] : []
                : [];

            snapScreen.IsActive = !Settings.DeactivedScreens.Contains(snapScreen.DeviceName);

            snapScreens.Add(snapScreen);
        }

        return snapScreens;
    }

    private void SaveApplicationGroupSettings()
    {
        fileOperationService.Save(ApplicationGroupSettings);
    }
}