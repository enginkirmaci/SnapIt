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
        private SnapAreaOld current;

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
                Screen.Base.WorkingArea.Left,
                Screen.Base.WorkingArea.Top,
                Screen.Base.WorkingArea.Width,
                Screen.Base.WorkingArea.Height);
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
            var snapArea = new SnapAreaOld
            {
                Theme = settingService.Settings.Theme,
                LayoutArea = Screen.Layout?.LayoutArea
            };

            Content = snapArea;
        }

        public void GenerateSnapAreaBoundries()
        {
            if (SnapAreaBoundries == null)
            {
                var generated = new List<Rectangle>();

                var rootSnapArea = Content as SnapAreaOld;
                rootSnapArea.GenerateSnapAreaBoundries(ref generated, Dpi);

                SnapAreaBoundries = generated.OrderBy(i => i.X).ThenBy(i => i.Y).ToList();
            }
        }

        public Rectangle SelectElementWithPoint(int x, int y)
        {
            if (IsVisible)
            {
                var Point2Window = PointFromScreen(new Point(x, y));

                var element = InputHitTest(Point2Window);

                if (element != null && element is DependencyObject)
                {
                    element = ((DependencyObject)element).FindParent<SnapAreaOld>();
                }

                if (current != null)
                {
                    if (current.IsMergedSnapArea)
                    {
                        var parent = current.ParentSnapArea;
                        if (parent == null)
                        {
                            parent = current;
                        }

                        var children = parent.FindChildren<SnapAreaOld>();
                        foreach (var child in children)
                        {
                            child.NormalStyle();
                        }
                    }
                    else
                    {
                        current.NormalStyle();
                    }
                }

                if (element != null && element is SnapAreaOld)
                {
                    var snapArea = current = (SnapAreaOld)element;

                    if ((element as SnapAreaOld).IsMergedSnapArea)
                    {
                        snapArea = ((SnapAreaOld)element).ParentSnapArea;

                        var children = snapArea.FindChildren<SnapAreaOld>();
                        foreach (var child in children)
                        {
                            child.OnHoverStyle();
                        }

                        return snapArea.ScreenSnapArea(Dpi);
                    }
                    else
                    {
                        snapArea = (SnapAreaOld)element;

                        snapArea.OnHoverStyle();

                        return snapArea.ScreenSnapArea(Dpi);
                    }
                }
            }

            return Rectangle.Empty;
        }
    }
}