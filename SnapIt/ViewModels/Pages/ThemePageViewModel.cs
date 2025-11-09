using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using SnapIt.Common.Mvvm;
using SnapIt.Controls;
using SnapIt.Services.Contracts;

namespace SnapIt.ViewModels.Pages;

public class ThemePageViewModel : ViewModelBase
{
    private readonly ISnapManager snapManager;
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private SnapAreaTheme theme;
    private BitmapImage backgroundImage;
    private bool openApplyChangesBar;
    private SnapArea? snapArea;

    public Layout Layout { get; set; }

    public SnapAreaTheme Theme
    { get => theme; set { SetProperty(ref theme, value); } }

    public BitmapImage BackgroundImage { get => backgroundImage; set => SetProperty(ref backgroundImage, value); }
    public bool OpenApplyChangesBar { get => openApplyChangesBar; set => SetProperty(ref openApplyChangesBar, value); }

    public DelegateCommand ApplyChangesCommand { get; }
    public DelegateCommand DiscardChangesCommand { get; }

    public ThemePageViewModel(
        ISnapManager snapManager,
        ISettingService settingService,
        IWinApiService winApiService)
    {
        this.snapManager = snapManager;
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
            Size = new SnapIt.Common.Graphics.Size(500, 200),
            LayoutLines =
            [
                new Line
                {
                    Point = new SnapIt.Common.Graphics.Point(150, 0),
                    Size = new SnapIt.Common.Graphics.Size(0, 200)
                },
                new Line
                {
                    Point = new SnapIt.Common.Graphics.Point(150, 100),
                    Size = new SnapIt.Common.Graphics.Size(350, 0),
                    SplitDirection = SplitDirection.Horizontal
                },
                new Line
                {
                    Point = new SnapIt.Common.Graphics.Point(325, 100),
                    Size = new SnapIt.Common.Graphics.Size(0, 100)
                }
            ],
            Theme = Theme
        };
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        await settingService.InitializeAsync();
        await winApiService.InitializeAsync();

        snapArea = ((Page)args.Source)?.FindChildren<SnapArea>().FirstOrDefault();

        snapArea?.OnHoverStyle();
    }

    private void Theme_ThemeChanged()
    {
        OpenApplyChangesBar = true;
        Theme = Theme.Copy();
        Theme.ThemeChanged += Theme_ThemeChanged;

        snapArea?.OnHoverStyle(false);
    }

    private void ApplyChanges()
    {
        if (!Dev.IsActive)
        {
            snapManager.Dispose();
            snapManager.InitializeAsync();
        }
    }
}