using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class WindowsViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private bool disableForModal;
        private string selectedApplication;
        private ObservableCollection<string> runningApplications;
        private ExcludedApplication selectedExcludedApplication;
        private ObservableCollection<ExcludedApplication> excludedApplications;
        private bool isRunningApplicationsDialogOpen;
        private bool isExcludeApplicationDialogOpen;
        private ObservableCollection<MatchRule> matchRules;

        public bool DisableForFullscreen
        { get => settingService.Settings.DisableForFullscreen; set { settingService.Settings.DisableForFullscreen = value; ApplyChanges(); } }

        public bool DisableForModal
        { get { disableForModal = settingService.Settings.DisableForModal; return disableForModal; } set { settingService.Settings.DisableForModal = value; SetProperty(ref disableForModal, value); ApplyChanges(); } }

        public ObservableCollection<string> RunningApplications { get => runningApplications; set => SetProperty(ref runningApplications, value); }
        public string SelectedApplication { get => selectedApplication; set => SetProperty(ref selectedApplication, value); }
        public ObservableCollection<ExcludedApplication> ExcludedApplications { get => excludedApplications; set => SetProperty(ref excludedApplications, value); }
        public ExcludedApplication SelectedExcludedApplication { get => selectedExcludedApplication; set => SetProperty(ref selectedExcludedApplication, value); }
        public bool IsExcludeApplicationDialogOpen { get => isExcludeApplicationDialogOpen; set => SetProperty(ref isExcludeApplicationDialogOpen, value); }
        public bool IsRunningApplicationsDialogOpen { get => isRunningApplicationsDialogOpen; set => SetProperty(ref isRunningApplicationsDialogOpen, value); }
        public ObservableCollection<MatchRule> MatchRules { get => matchRules; set => SetProperty(ref matchRules, value); }

        public DelegateCommand ExcludeWindowsModalCommand { get; private set; }
        public DelegateCommand NewExcludeApplicationCommand { get; private set; }
        public DelegateCommand OpenRunningApplicationsDialogCommand { get; private set; }
        public DelegateCommand CloseRunningApplicationsDialogCommand { get; private set; }
        public DelegateCommand<object> ExcludeApplicationDialogClosingCommand { get; private set; }
        public DelegateCommand<ExcludedApplication> RemoveExcludedApplicationCommand { get; private set; }
        public DelegateCommand<ExcludedApplication> EditExcludedApplicationCommand { get; private set; }

        public WindowsViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            if (settingService.ExcludedApplicationSettings?.Applications != null)
            {
                ExcludedApplications = new ObservableCollection<ExcludedApplication>(settingService.ExcludedApplicationSettings.Applications);
            }
            else
            {
                ExcludedApplications = new ObservableCollection<ExcludedApplication>();
            }

            ExcludeWindowsModalCommand = new DelegateCommand(() =>
            {
                if (!ExcludedApplications.Any(e => e.Keyword == "Action center"))
                {
                    ExcludedApplications.Add(new ExcludedApplication
                    {
                        Keyword = "Action center",
                        MatchRule = MatchRule.Contains,
                        Mouse = true,
                        Keyboard = true
                    });
                }

                if (!ExcludedApplications.Any(e => e.Keyword == "Action center"))
                {
                    ExcludedApplications.Add(new ExcludedApplication
                    {
                        Keyword = "Action center",
                        MatchRule = MatchRule.Contains,
                        Mouse = true,
                        Keyboard = true
                    });
                }

                if (!ExcludedApplications.Any(e => e.Keyword == "Start"))
                {
                    ExcludedApplications.Add(new ExcludedApplication
                    {
                        Keyword = "Start",
                        MatchRule = MatchRule.Contains,
                        Mouse = true,
                        Keyboard = true
                    });
                }
                if (!ExcludedApplications.Any(e => e.Keyword == "New notification"))
                {
                    ExcludedApplications.Add(new ExcludedApplication
                    {
                        Keyword = "New notification",
                        MatchRule = MatchRule.Contains,
                        Mouse = true,
                        Keyboard = true
                    });
                }

                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            });

            NewExcludeApplicationCommand = new DelegateCommand(() =>
            {
                var excludedApplication = new ExcludedApplication();

                SelectedExcludedApplication = excludedApplication;

                IsExcludeApplicationDialogOpen = true;
            });

            OpenRunningApplicationsDialogCommand = new DelegateCommand(() =>
            {
                IsRunningApplicationsDialogOpen = true;

                RunningApplications = new ObservableCollection<string>(winApiService.GetOpenWindowsNames());
            });

            CloseRunningApplicationsDialogCommand = new DelegateCommand(() =>
            {
                IsRunningApplicationsDialogOpen = false;
            });

            ExcludeApplicationDialogClosingCommand = new DelegateCommand<object>((isSave) =>
            {
                IsExcludeApplicationDialogOpen = false;

                if ((bool)isSave)
                {
                    if (!ExcludedApplications.Contains(SelectedExcludedApplication))
                    {
                        ExcludedApplications.Add(SelectedExcludedApplication);
                    }
                    else
                    {
                    }

                    settingService.SaveExcludedApps(ExcludedApplications.ToList());
                    ApplyChanges();
                }
            });

            RemoveExcludedApplicationCommand = new DelegateCommand<ExcludedApplication>((selected) =>
            {
                ExcludedApplications.Remove(selected);

                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            });

            EditExcludedApplicationCommand = new DelegateCommand<ExcludedApplication>((selected) =>
            {
                SelectedExcludedApplication = selected;

                IsExcludeApplicationDialogOpen = true;
            });

            MatchRules = new ObservableCollection<MatchRule> {
                MatchRule.Contains,
                MatchRule.Exact,
                MatchRule.Wildcard
            };
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