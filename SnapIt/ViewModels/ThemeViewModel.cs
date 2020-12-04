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
                LayoutArea = new LayoutArea
                {
                    Areas = new List<LayoutArea>
                    {
                        new LayoutArea
                        {
                            Width=1
                        },
                        new LayoutArea
                        {
                            Width=3,
                            Column=1,
                            Merged= true,
                            Areas = new List<LayoutArea>
                            {
                                new LayoutArea
                                {
                                    Height=1
                                },
                                new LayoutArea
                                {
                                    Height=1,
                                    Row=1,
                                    Areas = new List<LayoutArea>
                                    {
                                        new LayoutArea
                                        {
                                            Width=1
                                        },
                                        new LayoutArea
                                        {
                                            Width=1,
                                            Column=1
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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