﻿using System;
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

        public Entities.SnapScreen Screen { get; set; }
        public List<Rectangle> SnapAreaBoundries { get; set; }
        public Dpi Dpi { get; set; }

        public SnapWindow(
            ISettingService settingService,
            IWinApiService winApiService,
            Entities.SnapScreen screen)
        {
            this.settingService = settingService;
            this.winApiService = winApiService;

            Screen = screen;

            if (!DevMode.IsTopmostDisabled)
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

            CalculateDpi();
        }

        //TODO test here
        public new void Show()
        {
            base.Show();
            MaximizeWindow();
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

        private void CalculateDpi()
        {
            Dpi = Screen.GetDpi();
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
            }

            return Rectangle.Empty;
        }
    }
}