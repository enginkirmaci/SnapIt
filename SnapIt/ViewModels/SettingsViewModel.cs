using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private bool isStartupTaskActive;

        public bool DisableForFullscreen { get => settingService.Settings.DisableForFullscreen; set { settingService.Settings.DisableForFullscreen = value; ApplyChanges(); } }

        //public bool IsRunAsAdmin
        //{
        //	get => Properties.Settings.Default.RunAsAdmin;
        //	set
        //	{
        //		Properties.Settings.Default.RunAsAdmin = value;
        //		Properties.Settings.Default.Save();
        //	}
        //}

        public bool IsStartupTaskActive
        {
            get => isStartupTaskActive;
            set
            {
                SetProperty(ref isStartupTaskActive, value);
                settingService.SetStartupTaskStatusAsync(value);
            }
        }

        public DelegateCommand LoadedCommand { get; private set; }

        public SettingsViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            LoadedCommand = new DelegateCommand(async () =>
            {
                IsStartupTaskActive = await settingService.GetStartupTaskStatusAsync();
            });
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