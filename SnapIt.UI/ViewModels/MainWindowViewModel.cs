using System.Collections.ObjectModel;
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
        private ObservableCollection<MouseButton> mouseButtons;

        public string Title { get; set; } = $"Snap It {System.Windows.Forms.Application.ProductVersion}";
        public bool DragByTitle { get => config.DragByTitle; set { config.DragByTitle = value; ApplyChanges(); } }
        public MouseButton MouseButton { get => config.MouseButton; set { config.MouseButton = value; ApplyChanges(); } }
        public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }

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
            CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);

            config = configService.Load<Config>();
            MouseButtons = new ObservableCollection<MouseButton>
            {
                MouseButton.Left,
                MouseButton.Middle,
                MouseButton.Right
            };

            snapService.Initialize();
        }

        private void ApplyChanges()
        {
            configService.Save(config);

            snapService.Release();
            snapService.Initialize();
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                configService.Save(config);

                window.Hide();
            }

            if (Settings.IsDevMode)
            {
                Application.Current.Shutdown();
            }
        }
    }
}