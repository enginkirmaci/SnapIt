using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Configuration;
using SnapIt.Entities;
using SnapIt.Services;
using SnapIt.UI.Views;

namespace SnapIt.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly IConfigService configService;

        private Config config;
        private ObservableCollection<MouseButton> mouseButtons;
        private ObservableCollection<SnapScreen> snapScreens;
        private SnapScreen _selectedSnapScreens;
        private ObservableCollection<SnapLayout> snapLayouts;
        private SnapLayout selectedSnapLayout;

        public string Title { get; set; } = $"Snap It {System.Windows.Forms.Application.ProductVersion}";
        public bool DragByTitle { get => config.DragByTitle; set { config.DragByTitle = value; ApplyChanges(); } }
        public MouseButton MouseButton { get => config.MouseButton; set { config.MouseButton = value; ApplyChanges(); } }
        public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
        public bool DisableForFullscreen { get => config.DisableForFullscreen; set { config.DisableForFullscreen = value; ApplyChanges(); } }
        public ObservableCollection<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }
        public SnapScreen SelectedSnapScreen { get => _selectedSnapScreens; set => SetProperty(ref _selectedSnapScreens, value); }
        public ObservableCollection<SnapLayout> SnapLayouts { get => snapLayouts; set => SetProperty(ref snapLayouts, value); }
        public SnapLayout SelectedSnapLayout
        {
            get => selectedSnapLayout;
            set
            {
                SetProperty(ref selectedSnapLayout, value);
                SaveLayoutCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand NewLayoutCommand { get; private set; }
        public DelegateCommand SaveLayoutCommand { get; private set; }

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

            NewLayoutCommand = new DelegateCommand(() =>
            {
                SelectedSnapLayout = new SnapLayout
                {
                    Guid = Guid.NewGuid()
                };
            });

            SaveLayoutCommand = new DelegateCommand(() =>
            {
                System.Windows.MessageBox.Show("ok");
            },
            () =>
            {
                return !string.IsNullOrWhiteSpace(SelectedSnapLayout?.Name);
            }).ObservesProperty(() => SelectedSnapLayout.Name);

            config = configService.Load<Config>();
            MouseButtons = new ObservableCollection<MouseButton>
            {
                MouseButton.Left,
                MouseButton.Middle,
                MouseButton.Right
            };

            //if (!DevMode.IsActive)
            //{
            //    snapService.Initialize();
            //}
            //else
            //{
            //    //var test = new TestWindow();
            //    //test.Show();
            //    OpenDesigner();
            //}

            SnapScreens = new ObservableCollection<SnapScreen>();

            foreach (var screen in Screen.AllScreens)
            {
                SnapScreens.Add(new SnapScreen(screen));
            }

            SelectedSnapScreen = SnapScreens.FirstOrDefault(screen => screen.Base.Primary);

            SnapLayouts = new ObservableCollection<SnapLayout>();
            SnapLayouts.Add(new SnapLayout { Name = "Test" });
            SnapLayouts.Add(new SnapLayout { Name = "Test 2" });
            SnapLayouts.Add(new SnapLayout { Name = "Test 3" });
            SnapLayouts.Add(new SnapLayout { Name = "Test 4" });
            SnapLayouts.Add(new SnapLayout { Name = "Test 5" });
        }

        private void OpenDesigner()
        {
            foreach (var screen in Screen.AllScreens)
            {
                var designWindow = new DesignWindow();
                designWindow.SetScreen(screen);
                designWindow.Show();
            }
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

            if (DevMode.IsActive)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}