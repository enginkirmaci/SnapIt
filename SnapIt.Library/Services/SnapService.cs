using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Mappers;

namespace SnapIt.Library.Services
{
    public class SnapService : ISnapService
    {
        private readonly IWindowService windowService;
        private readonly ISettingService settingService;

        private ActiveWindow ActiveWindow;
        private Rectangle snapArea;
        private bool isWindowDetected = false;
        private bool isListening = false;
        private IKeyboardMouseEvents globalHook;

        public event GetStatus StatusChanged;

        public SnapService(
            IWindowService windowService,
            ISettingService settingService)
        {
            this.windowService = windowService;
            this.settingService = settingService;
        }

        public void Initialize()
        {
            windowService.Initialize();
            windowService.EscKeyPressed += WindowService_EscKeyPressed;

            var map = new Dictionary<Combination, Action>
            {
                {Combination.FromString("LWin+Left"), ()=> MoveActiveWindowByKeyboard(MoveDirection.Left) },
                {Combination.FromString("LWin+Right"), ()=> MoveActiveWindowByKeyboard(MoveDirection.Right) },
                {Combination.FromString("Control+Alt+Left"), ()=> MoveActiveWindowByKeyboard(MoveDirection.Left) },
                {Combination.FromString("Control+Alt+Right"), ()=> MoveActiveWindowByKeyboard(MoveDirection.Right) },
                {Combination.FromString("Control+Alt+Up"), ()=> MoveActiveWindowByKeyboard(MoveDirection.Up) },
                {Combination.FromString("Control+Alt+Down"), ()=> MoveActiveWindowByKeyboard(MoveDirection.Down) }
            };

            globalHook = Hook.GlobalEvents();

            if (settingService.Settings.EnableKeyboard)
            {
                globalHook.OnCombination(map);
            }

            if (settingService.Settings.EnableMouse)
            {
                globalHook.MouseMove += MouseMoveEvent;
                globalHook.MouseDown += MouseDownEvent;
                globalHook.MouseUp += MouseUpEvent;
            }

            StatusChanged?.Invoke(true);
        }

        private void MoveActiveWindowByKeyboard(MoveDirection direction)
        {
            ActiveWindow = User32Test.GetActiveWindow();

            if (ActiveWindow != ActiveWindow.Empty)
            {
                if (settingService.Settings.DisableForFullscreen && User32Test.IsFullscreen(ActiveWindow.Boundry) ||
                IsExcludedApplication(ActiveWindow.Title))
                {
                    return;
                }

                var boundries = windowService.SnapAreaBoundries();
                if (boundries != null)
                {
                    User32Test.GetWindowMargin(ActiveWindow, out Rectangle rectmargin);
                    var activeBoundry = boundries.FirstOrDefault(i => i.Contains(rectmargin));
                    var copyActiveBoundry = new Rectangle(activeBoundry.Left, activeBoundry.Top, activeBoundry.Right, activeBoundry.Bottom);

                    ActiveWindow.Dpi = DpiHelper.GetDpiFromPoint(activeBoundry.Left, activeBoundry.Right);

                    switch (direction)
                    {
                        case MoveDirection.Left:
                            activeBoundry.Left -= 1;
                            break;

                        case MoveDirection.Right:
                            activeBoundry.Left += activeBoundry.Width + 1;
                            break;

                        case MoveDirection.Up:
                            activeBoundry.Top -= 1;
                            break;

                        case MoveDirection.Down:
                            activeBoundry.Top += activeBoundry.Height + 1;
                            break;
                    }

                    var newSnapArea = boundries.FirstOrDefault(i => i.Dpi.Equals(ActiveWindow.Dpi) ? i.Contains(activeBoundry) : i.ContainsDpiAwareness(activeBoundry));

                    if (newSnapArea.Equals(Rectangle.Empty))
                    {
                        newSnapArea = copyActiveBoundry;
                    }

                    MoveActiveWindow(!newSnapArea.Equals(Rectangle.Empty) ? newSnapArea : copyActiveBoundry, true);
                }
            }
        }

        private void WindowService_EscKeyPressed()
        {
            windowService.Hide();
            isListening = false;
        }

        public void Release()
        {
            windowService.Release();

            if (globalHook != null)
            {
                globalHook.MouseMove -= MouseMoveEvent;
                globalHook.MouseDown -= MouseDownEvent;
                globalHook.MouseUp -= MouseUpEvent;
                globalHook.Dispose();
            }

            StatusChanged?.Invoke(false);
        }

        private bool IsExcludedApplication(string Title)
        {
            if (settingService.ExcludedApps?.Applications != null)
            {
                return settingService.ExcludedApps.Applications.Any(i => Title.Contains(i));
            }

            return false;
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (isListening)
            {
                if (!isWindowDetected)
                {
                    ActiveWindow = User32Test.GetActiveWindow();
                    ActiveWindow.Dpi = DpiHelper.GetDpiFromPoint(e.X, e.Y);

                    if (ActiveWindow?.Title != null && IsExcludedApplication(ActiveWindow.Title))
                    {
                        isListening = false;
                    }
                    else if (settingService.Settings.DisableForFullscreen && User32Test.IsFullscreen(ActiveWindow.Boundry))
                    {
                        isListening = false;
                    }
                    else if (settingService.Settings.DragByTitle)
                    {
                        var titleBarHeight = SystemInformation.CaptionHeight;
                        var FixedFrameBorderSize = SystemInformation.FixedFrameBorderSize.Height;

                        if (ActiveWindow.Boundry.Top + (titleBarHeight + 2 + FixedFrameBorderSize * 2) / ActiveWindow.Dpi.Y >= e.Location.Y)
                        {
                            isWindowDetected = true;
                        }
                        else
                        {
                            isListening = false;
                        }
                    }
                    else
                    {
                        isWindowDetected = true;
                    }
                }
                else if (!windowService.IsVisible)
                {
                    windowService.Show();
                }
                else
                {
                    snapArea = windowService.SelectElementWithPoint(e.Location.X, e.Location.Y);
                }
            }
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtonMapper.Map(settingService.Settings.MouseButton))
            {
                ActiveWindow = ActiveWindow.Empty;
                snapArea = Rectangle.Empty;
                isWindowDetected = false;
                isListening = true;
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtonMapper.Map(settingService.Settings.MouseButton) && isListening)
            {
                isListening = false;
                windowService.Hide();

                MoveActiveWindow(snapArea);
            }
        }

        private void MoveActiveWindow(Rectangle rectangle, bool usingKeyboard = false)
        {
            if (ActiveWindow != ActiveWindow.Empty)
            {
                if (!rectangle.Equals(Rectangle.Empty))
                {
                    if (!usingKeyboard)
                    {
                        SendKeys.SendWait("{ESC}");
                    }

                    User32Test.GetWindowMargin(ActiveWindow, out Rectangle withMargin);

                    if (!withMargin.Equals(default(Rectangle)))
                    {
                        var systemMargin = new Rectangle
                        {
                            Left = withMargin.Left - ActiveWindow.Boundry.Left,
                            Top = withMargin.Top - ActiveWindow.Boundry.Top,
                            Right = ActiveWindow.Boundry.Right - withMargin.Right,
                            Bottom = ActiveWindow.Boundry.Bottom - withMargin.Bottom
                        };

                        rectangle.Left -= systemMargin.Left;
                        rectangle.Top -= systemMargin.Top;
                        rectangle.Right += systemMargin.Right;
                        rectangle.Bottom += systemMargin.Bottom;
                    }

                    User32Test.MoveWindow(ActiveWindow, rectangle);

                    if (!rectangle.Dpi.Equals(ActiveWindow.Dpi))
                    {
                        User32Test.MoveWindow(ActiveWindow, rectangle);
                    }
                }
            }
        }
    }
}