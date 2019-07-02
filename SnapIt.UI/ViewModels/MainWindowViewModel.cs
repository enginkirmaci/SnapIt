using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Configuration;
using SnapIt.Entities;
using SnapIt.Services;

namespace SnapIt.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly IConfigService configService;

        private Config config;

        public string Title { get; set; } = $"Snap It {System.Windows.Forms.Application.ProductVersion}";
        public bool DragByTitle
        {
            get => config.DragByTitle;
            set
            {
                config.DragByTitle = value;
            }
        }

        public DelegateCommand<Window> CloseWindowCommand { get; private set; }

        public MainWindowViewModel(
            ISnapService snapService,
            IConfigService configService)
        {
            this.snapService = snapService;
            this.configService = configService;

            Initialize();
        }

        private void Initialize()
        {
            config = configService.Load<Config>();

            snapService.Initialize();

            CloseWindowCommand = new DelegateCommand<Window>((window) =>
            {
                configService.Save(config);

                if (window != null)
                {
                    window.Hide();
                }

                if (Settings.IsDevMode)
                {
                    Application.Current.Shutdown();
                }
            });
        }
    }
}