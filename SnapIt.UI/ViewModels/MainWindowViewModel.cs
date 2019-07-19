using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Configuration;
using SnapIt.Controls;
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
        private ObservableCollection<Layout> _layouts;
        private Layout selectedLayout;

        public string Title { get; set; } = $"Snap It {System.Windows.Forms.Application.ProductVersion}";
        public bool DragByTitle { get => config.DragByTitle; set { config.DragByTitle = value; ApplyChanges(); } }
        public MouseButton MouseButton { get => config.MouseButton; set { config.MouseButton = value; ApplyChanges(); } }
        public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
        public bool DisableForFullscreen { get => config.DisableForFullscreen; set { config.DisableForFullscreen = value; ApplyChanges(); } }
        public ObservableCollection<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }
        public SnapScreen SelectedSnapScreen { get => _selectedSnapScreens; set => SetProperty(ref _selectedSnapScreens, value); }
        public ObservableCollection<Layout> Layouts { get => _layouts; set => SetProperty(ref _layouts, value); }
        public Layout SelectedLayout
        {
            get => selectedLayout;
            set
            {
                SetProperty(ref selectedLayout, value);
                SaveLayoutCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand NewLayoutCommand { get; private set; }
        public DelegateCommand SaveLayoutCommand { get; private set; }
        public DelegateCommand DesignLayoutCommand { get; private set; }

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
                SelectedLayout = new Layout
                {
                    Guid = Guid.NewGuid()
                };

                var designWindow = new DesignWindow();
                designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
                designWindow.Show();
            });

            SaveLayoutCommand = new DelegateCommand(() =>
            {
                selectedLayout.Save();
            },
            () =>
            {
                return !string.IsNullOrWhiteSpace(SelectedLayout?.Name);
            }).ObservesProperty(() => SelectedLayout.Name);

            DesignLayoutCommand = new DelegateCommand(() =>
            {
                var designWindow = new DesignWindow();
                designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
                designWindow.Show();
            },
            () =>
            {
                return SelectedLayout != null;
            }).ObservesProperty(() => SelectedLayout);

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

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Layouts");
            var files = Directory.GetFiles(folderPath, "*.json");
            Layouts = new ObservableCollection<Layout>();

            foreach (var file in files)
            {
                var layout = JsonConvert.DeserializeObject<Layout>(File.ReadAllText(file));
                Layouts.Add(layout);
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