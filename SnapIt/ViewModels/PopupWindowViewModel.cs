﻿using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Prism.Commands;
using Prism.Mvvm;
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
        private ApplicationGroup selectedApplicationGroup;
        private Window popupWindow;
        private bool isRunning;
        private string status;

        public ObservableCollectionWithItemNotify<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }

        public SnapScreen SelectedSnapScreen { get => selectedSnapScreen; set => SetProperty(ref selectedSnapScreen, value); }

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

        public PopupWindowViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
            this.winApiService = winApiService;

            snapService.StatusChanged += SnapService_StatusChanged;

            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens?.FirstOrDefault();

            LoadedCommand = new DelegateCommand<Window>((window) =>
            {
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

                //NavigateView(navigatePath);
            });

            //TODO here
        }

        private void PopupWindow_Deactivated(object? sender, System.EventArgs e)
        {
            try
            {
                popupWindow.Close();
            }
            catch { }
        }

        //private void SnapService_ScreenChanged(System.Collections.Generic.IList<SnapScreen> snapScreens)
        //{
        //    try
        //    {
        //        if (selectedSnapScreen != null)
        //        {
        //            var deviceNumber = selectedSnapScreen.DeviceNumber;

        //            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
        //            SelectedSnapScreen = SnapScreens.FirstOrDefault(s => s.DeviceNumber == deviceNumber);
        //        }
        //    }
        //    catch { }
        //}

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