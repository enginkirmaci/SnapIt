using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class ThemeViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;

        private SnapAreaTheme theme;
        private BitmapImage backgroundImage;
        private SnapAreaOld snapArea;
        private bool openApplyChangesBar;

        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get => theme; set { SetProperty(ref theme, value); } }
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

            BackgroundImage = new BitmapImage(new Uri(winApiService.GetCurrentDesktopWallpaper()));

            Theme = settingService.Settings.Theme.Copy();

            Theme.ThemeChanged += Theme_ThemeChanged;

            LoadedCommand = new DelegateCommand<object>((mainSnapGrid) =>
            {
                var grid = mainSnapGrid as DependencyObject;

                snapArea = grid.FindChildren<SnapAreaOld>().FirstOrDefault();

                if (snapArea != null)
                {
                    snapArea.SetPreview();
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
                Size = new Size(1436, 700.8),
                LayoutLines = new List<LayoutLine>
                {
                    new LayoutLine
                    {
                        Point=new Point(259.904414003044,0),
                        Size = new Size(0,700.8)
                    },
                     new LayoutLine
                    {
                        Point=new Point( 1230.4596651446,0),
                        Size = new Size(0,700.8 )
                    },
                     new LayoutLine
                    {
                        Point=new Point(259.904414003044,471.072897196262 ),
                        Size = new Size(970.555251141553,0 ),
                        SplitDirection = SplitDirection.Horizontal
                    },
                     new LayoutLine
                    {
                        Point=new Point(455.331811263318,471.072897196262 ),
                        Size = new Size(0,229.727102803738 )
                    },
                     new LayoutLine
                    {
                        Point=new Point(567.733637747336,0 ),
                        Size = new Size( 0,471.072897196262)
                    },
                     new LayoutLine
                    {
                        Point=new Point(567,235.5 ),
                        Size = new Size(663,0 ),
                        SplitDirection = SplitDirection.Horizontal
                    },
                     new LayoutLine
                    {
                        Point=new Point(898.5,235 ),
                        Size = new Size( 0,236)
                    },
                     new LayoutLine
                    {
                        Point=new Point(842.5,471 ),
                        Size = new Size( 0,229)
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
                snapArea.SetPreview();
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