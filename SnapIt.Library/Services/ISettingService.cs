using System.Collections.Generic;
using System.Threading.Tasks;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface ISettingService
    {
        Settings Settings { get; }
        ExcludedApplicationSettings ExcludedApplicationSettings { get; }
        IList<Layout> Layouts { get; }
        IList<SnapScreen> SnapScreens { get; }

        void ReInitialize();

        void Save();

        void SaveLayout(Layout layout);

        void SaveExcludedApps(List<ExcludedApplication> excludedApplications);

        void ExportLayout(Layout layout, string layoutPath);

        void DeleteLayout(Layout layout);

        Layout ImportLayout(string layoutPath);

        void LinkScreenLayout(SnapScreen snapScreen, Layout layout);

        Task<bool> GetStartupTaskStatusAsync();

        Task SetStartupTaskStatusAsync(bool isActive);
    }
}