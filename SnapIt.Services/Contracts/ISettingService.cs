using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;

namespace SnapIt.Services.Contracts;

public interface ISettingService : IInitialize
{
    Settings Settings { get; }
    ExcludedApplicationSettings ExcludedApplicationSettings { get; }
    StandaloneLicense StandaloneLicense { get; }
    IList<Layout> Layouts { get; }
    IList<SnapScreen> SnapScreens { get; }
    SnapScreen LatestActiveScreen { get; set; }
    SnapScreen SelectedSnapScreen { get; set; }

    void LoadSettings();

    void ReInitialize();

    void Save();

    void SaveLayout(Layout layout);

    void SaveExcludedApps(List<ExcludedApplication> excludedApplications);

    void SaveStandaloneLicense(StandaloneLicense standaloneLicense);

    void ExportLayout(Layout layout, string layoutPath);

    void DeleteLayout(Layout layout);

    Layout ImportLayout(string layoutPath);

    void LinkScreenLayout(SnapScreen snapScreen, Layout layout);

    void LinkScreenApplicationGroups(SnapScreen snapScreen, List<ApplicationGroup> applicationGroups);

    Task<bool> GetStartupTaskStatusAsync();

    Task SetStartupTaskStatusAsync(bool isActive);
}