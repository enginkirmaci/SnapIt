//using ControlzEx.Theming;
using System.Windows;
using System.Windows.Media.Imaging;
using Prism.Commands;
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
    private SnapArea snapArea;

    public Layout Layout { get; set; }

    public SnapAreaTheme Theme
    { get => theme; set { SetProperty(ref theme, value); } }

    public BitmapImage BackgroundImage { get => backgroundImage; set => SetProperty(ref backgroundImage, value); }
    public bool OpenApplyChangesBar { get => openApplyChangesBar; set => SetProperty(ref openApplyChangesBar, value); }

    public DelegateCommand<object> LoadedCommand { get; }
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

    //<!--<ui:Snackbar
    //    Grid.Row="0"
    //    Grid.RowSpan= "2"
    //    Grid.Column= "1"
    //    Panel.ZIndex= "9"
    //    Appearance= "Secondary"
    //    Icon= "Empty"
    //    IsShown= "{Binding OpenApplyChangesBar, Mode=TwoWay}" >
    //    < i:Interaction.Triggers>
    //        <i:EventTrigger EventName = "ButtonCloseCommand" >
    //            < i:InvokeCommandAction Command = "{Binding DiscardChangesCommand}" CommandParameter= "{StaticResource FalseValue}" />
    //        </ i:EventTrigger>
    //    </i:Interaction.Triggers>

    //    <Grid>
    //        <Grid.ColumnDefinitions>
    //            <ColumnDefinition Width = "*" />
    //            < ColumnDefinition Width= "Auto" />
    //        </ Grid.ColumnDefinitions >

    //        < TextBlock
    //            VerticalAlignment= "Center"
    //            FontWeight= "Medium"
    //            Text= "Do you want to apply changes?" />
    //        < ui:Button
    //            Grid.Column= "1"
    //            Margin= "12,0,0,0"
    //            Command= "{Binding ApplyChangesCommand}"
    //            Content= "Apply Changes" />
    //    </ Grid >
    //</ ui:Snackbar>-->
    public override async Task InitializeAsync()
    {
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
        if (!Dev.IsActive)
        {
            snapManager.Release();
            snapManager.InitializeAsync();
        }
    }
}