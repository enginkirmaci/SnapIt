using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Services;

namespace SnapIt.Library.Controls
{
    public class SnapWindow : Window
    {
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;
        private SnapArea currentArea;
        private SnapOverlay currentOverlay;

        public SnapScreen Screen { get; set; }
        public List<Rectangle> SnapAreaBoundries { get; set; }
        public Dpi Dpi { get; set; }

        public SnapWindow(
            ISettingService settingService,
            IWinApiService winApiService,
            SnapScreen screen)
        {
            this.settingService = settingService;
            this.winApiService = winApiService;

            Screen = screen;

            Topmost = true;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Width = screen.Base.WorkingArea.Width;
            Height = screen.Base.WorkingArea.Height;
            Left = screen.Base.WorkingArea.X;
            Top = screen.Base.WorkingArea.Y;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;

            CalculateDpi();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            MaximizeWindow();
        }

        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);

        //    var oldDpi = Dpi;

        //    CalculateDpi();

        //    if (!oldDpi.Equals(Dpi))
        //    {
        //        MaximizeWindow();
        //    }
        //}

        private void MaximizeWindow()
        {
            var wih = new WindowInteropHelper(this);
            var window = new ActiveWindow
            {
                Handle = wih.Handle
            };

            winApiService.MoveWindow(
                window,
                (int)Screen.Base.WorkingArea.Left,
                (int)Screen.Base.WorkingArea.Top,
                (int)Screen.Base.WorkingArea.Width,
                (int)Screen.Base.WorkingArea.Height);
        }

        private void CalculateDpi()
        {
            Screen.Base.GetDpi(DpiType.Effective, out uint x, out uint y);

            Dpi = new Dpi
            {
                X = 96.0 / x,
                Y = 96.0 / y
            };
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
                var snapControl = Content as SnapControl;
                var snapAreas = snapControl.FindChildren<SnapArea>();
                var snapOverlays = snapControl.FindChildren<SnapOverlay>();

                var generated = snapAreas.Select(snapArea => snapArea.ScreenSnapArea(Dpi)).ToList();

                SnapAreaBoundries = generated.OrderBy(i => i.X).ThenBy(i => i.Y).ToList();
            }
        }

        public Rectangle SelectElementWithPoint(int x, int y)
        {
            if (IsVisible)
            {
                var Point2Window = PointFromScreen(new Point(x, y));

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

                if (currentArea != null)
                {
                    currentArea.NormalStyle();
                }
                if (currentOverlay != null)
                {
                    currentOverlay.NormalStyle();
                }

                if (dependencyObject != null)
                {
                    if (dependencyObject is SnapArea)
                    {
                        var snapArea = currentArea = (SnapArea)dependencyObject;

                        snapArea.OnHoverStyle();

                        return snapArea.ScreenSnapArea(Dpi);
                    }
                    if (dependencyObject is SnapOverlay)
                    {
                        var snapOverlay = currentOverlay = (SnapOverlay)dependencyObject;

                        snapOverlay.OnHoverStyle();

                        return snapOverlay.ScreenSnapArea(Dpi);
                    }
                }
            }

            return Rectangle.Empty;
        }
    }
}