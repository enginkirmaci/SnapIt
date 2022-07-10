using System.Collections.Generic;
using System.Threading.Tasks;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface ISettingService
    {
        Settings Settings { get; }
        ExcludedApplicationSettings ExcludedApplicationSettings { get; }
        StandaloneLicense StandaloneLicense { get; }
        IList<Layout> Layouts { get; }
        IList<SnapScreen> SnapScreens { get; }
        SnapScreen LatestActiveScreen { get; set; }
        SnapScreen SelectedSnapScreen { get; set; }

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
}