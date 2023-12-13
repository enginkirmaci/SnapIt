using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SnapIt.Common;
using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using SnapIt.Services.Contracts;

namespace SnapIt.Controls;

public class SnapWindow : Window, IWindow
{
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private SnapArea currentArea;
    private SnapOverlay currentOverlay;

    public SnapScreen Screen { get; set; }
    public List<Rectangle> SnapAreaBoundries { get; set; }
    public Dictionary<int, Rectangle> SnapAreaRectangles { get; set; }
    public Dpi Dpi { get; set; }

    public SnapWindow(
        ISettingService settingService,
        IWinApiService winApiService,
        SnapScreen screen)
    {
        this.settingService = settingService;
        this.winApiService = winApiService;

        Screen = screen;

        if (!Dev.IsTopmostDisabled)
        {
            Topmost = true;
        }

        AllowsTransparency = true;
        Background = new SolidColorBrush(Colors.Transparent);
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        Width = screen.WorkingArea.Width;
        Height = screen.WorkingArea.Height;
        Left = screen.WorkingArea.X;
        Top = screen.WorkingArea.Y;
        WindowState = WindowState.Normal;
        WindowStyle = WindowStyle.None;

        Dpi = new Dpi()
        {
            X = (float)(100 / (screen.ScaleFactor * 100)),
            Y = (float)(100 / (screen.ScaleFactor * 100))
        };
    }

    //TODO test here
    public new void Show()
    {
        base.Show();
        //MaximizeWindow();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        MaximizeWindow();
    }

    private void MaximizeWindow()
    {
        var wih = new WindowInteropHelper(this);
        var window = new ActiveWindow
        {
            Handle = wih.Handle
        };

        winApiService.MoveWindow(
            window,
            (int)Screen.WorkingArea.Left,
            (int)Screen.WorkingArea.Top,
            (int)Screen.WorkingArea.Width,
            (int)Screen.WorkingArea.Height);
    }

    public void ApplyLayout()
    {
        var snapControl = new SnapControl
        {
            Theme = settingService.Settings.Theme,
            Layout = Screen.Layout
        };

        Content = snapControl;
    }

    public void GenerateSnapAreaBoundries()
    {
        if (SnapAreaBoundries == null)
        {
            SnapAreaBoundries = [];
            SnapAreaRectangles = [];

            var snapControl = Content as SnapControl;
            var snapAreas = snapControl.FindChildren<SnapArea>();
            var snapOverlays = snapControl.FindChildren<SnapOverlay>();

            foreach (var snapOverlay in snapOverlays)
            {
                SnapAreaRectangles.Add(snapOverlay.AreaNumber, snapOverlay.ScreenSnapArea(Dpi));
            }

            foreach (var snapArea in snapAreas)
            {
                var rectangle = snapArea.ScreenSnapArea(Dpi);

                SnapAreaRectangles.Add(snapArea.AreaNumber, rectangle);
                SnapAreaBoundries.Add(rectangle);
            }

            SnapAreaBoundries = SnapAreaBoundries.OrderBy(i => i.X).ThenBy(i => i.Y).ToList();
        }
    }

    public new void Hide()
    {
        //currentArea?.NormalStyle(false);
        //currentOverlay?.NormalStyle(false);

        base.Hide();
    }

    public Rectangle SelectElementWithPoint(int x, int y)
    {
        if (IsVisible)
        {
            var Point2Window = PointFromScreen(new System.Windows.Point(x, y));

            var element = InputHitTest(Point2Window);

            DependencyObject dependencyObject = null;
            if (element != null && element is DependencyObject)
            {
                dependencyObject = ((DependencyObject)element).FindParent<SnapArea>();
                if (dependencyObject == null)
                {
                    dependencyObject = ((DependencyObject)element).FindParent<SnapOverlay>();
                }
            }

            if (dependencyObject is SnapArea && currentArea?.Name != ((SnapArea)dependencyObject).Name)
            {
                currentArea?.NormalStyle();
                currentOverlay?.NormalStyle();
            }
            else if (dependencyObject is not SnapArea)
            {
                currentArea?.NormalStyle();
            }

            if (dependencyObject is SnapOverlay && currentOverlay?.Name != ((SnapOverlay)dependencyObject).Name)
            {
                currentArea?.NormalStyle();
                currentOverlay?.NormalStyle();
            }
            else if (dependencyObject is not SnapOverlay)
            {
                currentOverlay?.NormalStyle();
            }

            if (dependencyObject != null)
            {
                if (dependencyObject is SnapArea)
                {
                    var snapArea = (SnapArea)dependencyObject;

                    if (!(currentArea != null && currentArea.Name == ((SnapArea)dependencyObject).Name))
                    {
                        snapArea.OnHoverStyle();
                    }

                    currentArea = snapArea;
                    currentOverlay = null;

                    return snapArea.ScreenSnapArea(Dpi);
                }

                if (dependencyObject is SnapOverlay)
                {
                    var snapOverlay = (SnapOverlay)dependencyObject;

                    if (!(currentOverlay != null && currentOverlay?.Name == ((SnapOverlay)dependencyObject).Name))
                    {
                        snapOverlay.OnHoverStyle();
                    }

                    currentArea = null;
                    currentOverlay = snapOverlay;

                    return snapOverlay.ScreenSnapArea(Dpi);
                }
            }
            else
            {
                currentArea = null;
                currentOverlay = null;
            }
        }

        return Rectangle.Empty;
    }
}