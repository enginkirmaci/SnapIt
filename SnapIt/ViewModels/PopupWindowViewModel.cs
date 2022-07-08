﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class PopupWindowViewModel : BindableBase
    {
        private const int MARGIN = 20;
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;

        private SnapAreaTheme theme = new SnapAreaTheme();
        private ObservableCollectionWithItemNotify<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private ObservableCollection<Layout> layouts;
        private Layout selectedLayout;
        private ApplicationGroup selectedApplicationGroup;
        private Window popupWindow;
        private bool isRunning;
        private string status;
        private bool isSelectedScreenChanged;

        public ObservableCollectionWithItemNotify<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }

        public SnapScreen SelectedSnapScreen
        {
            get => selectedSnapScreen;
            set
            {
                SetProperty(ref selectedSnapScreen, value);
                isSelectedScreenChanged = true;
                SelectedLayout = selectedSnapScreen?.Layout;
            }
        }

        public ObservableCollection<Layout> Layouts { get => layouts; set => SetProperty(ref layouts, value); }

        public Layout SelectedLayout
        {
            get => selectedLayout;
            set
            {
                SetProperty(ref selectedLayout, value);

                if (!isSelectedScreenChanged && value != null)
                {
                    SelectedSnapScreen.Layout = selectedLayout;
                    settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);
                    ApplyChanges();
                }

                isSelectedScreenChanged = false;
            }
        }

        public ApplicationGroup SelectedApplicationGroup
        {
            get => selectedApplicationGroup; set
            {
                snapService.StartApplications(SelectedSnapScreen, value);

                popupWindow.Close();

                SetProperty(ref selectedApplicationGroup, value);
            }
        }

        public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }
        public string Status { get => status; set => SetProperty(ref status, value); }
        public DelegateCommand<Window> LoadedCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand StartStopCommand { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }

        private async Task TestLoadingWindow()
        {
            DevMode.Log("started");
            var loadingWindow = new SnapLoadingWindow(winApiService, SelectedSnapScreen);
            loadingWindow.Show();

            DevMode.Log("test 1");
            loadingWindow.SetLoadingMessage("Test 1");
            await Task.Delay(1000);

            DevMode.Log("test 2");
            loadingWindow.LoadingMessage = "Test 22";
            await Task.Delay(1000);

            DevMode.Log("test 3");
            loadingWindow.LoadingMessage = "Test 333";
            await Task.Delay(1000);

            DevMode.Log("test 4");
            loadingWindow.LoadingMessage = "Test 4444";
            await Task.Delay(1000);

            loadingWindow.Hide();
        }

        public PopupWindowViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
            this.winApiService = winApiService;

            snapService.StatusChanged += SnapService_StatusChanged;

            Layouts = new ObservableCollection<Layout>(settingService.Layouts);
            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens?.FirstOrDefault();

            LoadedCommand = new DelegateCommand<Window>((window) =>
            {
                TestLoadingWindow();

                popupWindow = window;

                popupWindow.Deactivated += PopupWindow_Deactivated;

                var wih = new WindowInteropHelper(popupWindow);
                var activeWindow = new ActiveWindow
                {
                    Handle = wih.Handle
                };

                var screen = SnapScreens.FirstOrDefault(i => i.IsPrimary);

                if (screen != null)
                {
                    var workingArea = screen.WorkingArea;

                    winApiService.MoveWindow(activeWindow,
                                         (int)(workingArea.Right - MARGIN - popupWindow.Width * screen.ScaleFactor),
                                         (int)(workingArea.Bottom - MARGIN - popupWindow.Height * screen.ScaleFactor),
                                         (int)workingArea.Width,
                                         (int)workingArea.Height);
                }

                SnapService_StatusChanged(snapService.IsRunning);
            });

            CloseCommand = new DelegateCommand(() =>
            {
                popupWindow.Close();
            });

            StartStopCommand = new DelegateCommand(() =>
            {
                if (snapService.IsRunning)
                {
                    snapService.Release();
                }
                else
                {
                    snapService.Initialize();
                }
            });

            NavigateCommand = new DelegateCommand<string>((navigatePath) =>
            {
                var mainWindow = Application.Current.MainWindow;

                if (mainWindow != null && mainWindow.IsVisible)
                {
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                    }

                    mainWindow.Activate();
                }
                else
                {
                    mainWindow.Show();

                    mainWindow.Activate();
                }

                ((MainWindowViewModel)mainWindow.DataContext).NavigateView(navigatePath);
            });
        }

        private void PopupWindow_Deactivated(object? sender, System.EventArgs e)
        {
            try
            {
                popupWindow.Close();
            }
            catch { }
        }

        private void ApplyChanges()
        {
            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }

        private void SnapService_StatusChanged(bool isRunning)
        {
            IsRunning = isRunning;

            if (isRunning)
            {
                Status = "Stop";
            }
            else
            {
                Status = "Start";
            }
        }
    }
}