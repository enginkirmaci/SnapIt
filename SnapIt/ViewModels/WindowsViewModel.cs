using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class WindowsViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;

        private ObservableCollection<string> runningApplications;
        private string selectedApplication;
        private string selectedExcludedApplication;
        private ObservableCollection<string> excludedApplications;

        public ObservableCollection<string> RunningApplications { get => runningApplications; set => SetProperty(ref runningApplications, value); }
        public string SelectedApplication { get => selectedApplication; set => SetProperty(ref selectedApplication, value); }
        public ObservableCollection<string> ExcludedApplications { get => excludedApplications; set => SetProperty(ref excludedApplications, value); }
        public string SelectedExcludedApplication { get => selectedExcludedApplication; set => SetProperty(ref selectedExcludedApplication, value); }

        public DelegateCommand ExcludeAppLayoutCommand { get; private set; }
        public DelegateCommand IncludeAppLayoutCommand { get; private set; }

        public WindowsViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
            this.winApiService = winApiService;
            RunningApplications = new ObservableCollection<string>(winApiService.GetOpenWindowsNames());
            if (settingService.ExcludedApps?.Applications != null)
            {
                ExcludedApplications = new ObservableCollection<string>(settingService.ExcludedApps.Applications);
            }
            else
            {
                ExcludedApplications = new ObservableCollection<string>();
            }

            ExcludeAppLayoutCommand = new DelegateCommand(() =>
            {
                ExcludedApplications.Add(SelectedApplication);
                SelectedApplication = null;

                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            },
            () =>
            {
                return !string.IsNullOrWhiteSpace(SelectedApplication);
            }).ObservesProperty(() => SelectedApplication);

            IncludeAppLayoutCommand = new DelegateCommand(() =>
            {
                ExcludedApplications.Remove(SelectedExcludedApplication);

                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            },
            () =>
            {
                return SelectedExcludedApplication != null;
            }).ObservesProperty(() => SelectedExcludedApplication);
        }

        private void ApplyChanges()
        {
            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }
    }
}