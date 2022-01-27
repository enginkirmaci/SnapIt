//using ControlzEx.Theming;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SnapIt.ViewModels
{
    public class ThemeViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;

        private SnapAreaTheme theme;
        private BitmapImage backgroundImage;
        private bool openApplyChangesBar;
        private SnapArea snapArea;

        public Layout Layout { get; set; }

        public SnapAreaTheme Theme
        { get => theme; set { SetProperty(ref theme, value); } }

        public BitmapImage BackgroundImage { get => backgroundImage; set => SetProperty(ref backgroundImage, value); }
        public bool OpenApplyChangesBar { get => openApplyChangesBar; set => SetProperty(ref openApplyChangesBar, value); }

        public DelegateCommand<object> LoadedCommand { get; }
        public DelegateCommand ApplyChangesCommand { get; }
        public DelegateCommand DiscardChangesCommand { get; }

        public ThemeViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
            this.winApiService = winApiService;

            try
            {
                BackgroundImage = new BitmapImage(new Uri(winApiService.GetCurrentDesktopWallpaper()));
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex, "Background Image handled exception");
            }

            Theme = settingService.Settings.Theme.Copy();

            Theme.ThemeChanged += Theme_ThemeChanged;

            LoadedCommand = new DelegateCommand<object>((mainSnapGrid) =>
            {
                var grid = mainSnapGrid as DependencyObject;

                snapArea = grid?.FindChildren<SnapArea>().FirstOrDefault();

                if (snapArea != null)
                {
                    snapArea.OnHoverStyle();
                }
            });

            ApplyChangesCommand = new DelegateCommand(() =>
            {
                settingService.Settings.Theme = Theme;
                ApplyChanges();
                settingService.Save();
                OpenApplyChangesBar = false;
            });

            DiscardChangesCommand = new DelegateCommand(() =>
            {
                Theme = settingService.Settings.Theme;
                ApplyChanges();
                Theme_ThemeChanged();
                OpenApplyChangesBar = false;
            });

            Layout = new Layout
            {
                Name = "Layout 1",
                Size = new Size(500, 200),
                LayoutLines = new List<LayoutLine>
                {
                    new LayoutLine
                    {
                        Point=new Point(150,0),
                        Size = new Size(0,200)
                    },
                     new LayoutLine
                    {
                        Point=new Point(150,100),
                        Size = new Size(350,0),
                        SplitDirection = SplitDirection.Horizontal
                    },
                     new LayoutLine
                    {
                        Point=new Point(325,100),
                        Size = new Size(0,100)
                    }
                },
                Theme = Theme
            };
        }

        private void Theme_ThemeChanged()
        {
            OpenApplyChangesBar = true;
            Theme = Theme.Copy();
            Theme.ThemeChanged += Theme_ThemeChanged;

            if (snapArea != null)
            {
                snapArea.OnHoverStyle();
            }
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