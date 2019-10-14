using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IWinApiService winApiService;

        private ActiveWindow ActiveWindow;
        private Rectangle snapArea;
        private bool isWindowDetected = false;
        private bool isListening = false;
        private bool isHoldingKey = false;
        private bool holdKeyUsed = false;
        private DateTime delayStartTime;
        private IKeyboardMouseEvents globalHook;

        public event GetStatus StatusChanged;

        public SnapService(
            IWindowService windowService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.windowService = windowService;
            this.settingService = settingService;
            this.winApiService = winApiService;
        }

        public void Initialize()
        {
            isWindowDetected = false;
            isListening = false;

            windowService.Initialize();
            windowService.EscKeyPressed += WindowService_EscKeyPressed;

            var map = new Dictionary<Combination, Action>
            {
                {Combination.FromString(settingService.Settings.MoveLeftShortcut.Replace(" ", string.Empty)), ()=> MoveActiveWindowByKeyboard(MoveDirection.Left) },
                {Combination.FromString(settingService.Settings.MoveRightShortcut.Replace(" ", string.Empty)), ()=> MoveActiveWindowByKeyboard(MoveDirection.Right) },
                {Combination.FromString(settingService.Settings.MoveUpShortcut.Replace(" ", string.Empty)), ()=> MoveActiveWindowByKeyboard(MoveDirection.Up) },
                {Combination.FromString(settingService.Settings.MoveDownShortcut.Replace(" ", string.Empty)), ()=> MoveActiveWindowByKeyboard(MoveDirection.Down) }
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

                if (settingService.Settings.EnableHoldKey)
                {
                    globalHook.KeyDown += GlobalHook_KeyDown;
                    globalHook.KeyUp += GlobalHook_KeyUp;
                }
            }

            StatusChanged?.Invoke(true);
        }

        private bool HoldingKeyResult()
        {
            if (settingService.Settings.EnableHoldKey)
            {
                if (isHoldingKey)
                {
                    switch (settingService.Settings.HoldKeyBehaviour)
                    {
                        case HoldKeyBehaviour.HoldToEnable:
                            return true;

                        case HoldKeyBehaviour.HoldToDisable:
                            StopSnapping();
                            return false;
                    }
                }
                else
                {
                    switch (settingService.Settings.HoldKeyBehaviour)
                    {
                        case HoldKeyBehaviour.HoldToEnable:
                            return false;

                        case HoldKeyBehaviour.HoldToDisable:
                            return true;
                    }
                }
            }

            return true;
        }

        public void Release()
        {
            windowService.Release();

            if (globalHook != null)
            {
                globalHook.MouseMove -= MouseMoveEvent;
                globalHook.MouseDown -= MouseDownEvent;
                globalHook.MouseUp -= MouseUpEvent;

                if (settingService.Settings.EnableHoldKey)
                {
                    globalHook.KeyDown -= GlobalHook_KeyDown;
                    globalHook.KeyUp -= GlobalHook_KeyUp;
                }

                globalHook.Dispose();
            }

            StatusChanged?.Invoke(false);
        }

        private void GlobalHook_KeyUp(object sender, KeyEventArgs e)
        {
            switch (settingService.Settings.HoldKey)
            {
                case HoldKey.Control:
                    if (e.KeyCode == Keys.Control || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
                    {
                        isHoldingKey = false;
                    }

                    break;

                case HoldKey.Alt:
                    if (e.KeyCode == Keys.Alt || e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
                    {
                        isHoldingKey = false;
                    }

                    break;

                case HoldKey.Shift:
                    if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                    {
                        isHoldingKey = false;
                    }

                    break;

                case HoldKey.Win:
                    if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                    {
                        isHoldingKey = false;

                        if (holdKeyUsed)
                        {
                            e.Handled = true;
                        }
                    }

                    break;
            }

            if (holdKeyUsed)
            {
                holdKeyUsed = false;
            }
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            switch (settingService.Settings.HoldKey)
            {
                case HoldKey.Control:
                    if (e.KeyCode == Keys.Control || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
                    {
                        isHoldingKey = true;
                    }

                    break;

                case HoldKey.Alt:
                    if (e.KeyCode == Keys.Alt || e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
                    {
                        isHoldingKey = true;
                    }

                    break;

                case HoldKey.Shift:
                    if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                    {
                        isHoldingKey = true;
                    }

                    break;

                case HoldKey.Win:
                    if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                    {
                        isHoldingKey = true;
                    }

                    break;
            }
        }

        private void MoveActiveWindowByKeyboard(MoveDirection direction)
        {
            ActiveWindow = winApiService.GetActiveWindow();

            if (ActiveWindow != ActiveWindow.Empty)
            {
                if ((settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(ActiveWindow.Boundry)) ||
                    IsExcludedApplication(ActiveWindow.Title))
                {
                    return;
                }

                var boundries = windowService.SnapAreaBoundries();
                if (boundries != null)
                {
                    winApiService.GetWindowMargin(ActiveWindow, out Rectangle rectmargin);
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

                    MoveActiveWindow(!newSnapArea.Equals(Rectangle.Empty) ? newSnapArea : copyActiveBoundry, false);
                }
            }
        }

        private void WindowService_EscKeyPressed()
        {
            StopSnapping();
        }

        private void StopSnapping()
        {
            windowService.Hide();
            isListening = false;
        }

        private bool IsExcludedApplication(string Title)
        {
            //if (settingService.ExcludedApplicationSettings?.Applications != null)
            //{
            //    return settingService.ExcludedApplicationSettings.Applications.Any(i => Title.Contains(i));
            //}

            return false;
        }

        private bool IsDelayDone()
        {
            if (settingService.Settings.EnableHoldKey)
                return true;

            var elapsedMillisecs = (DateTime.Now - delayStartTime).TotalMilliseconds;

            return elapsedMillisecs > settingService.Settings.MouseDragDelay;
        }

        private void MouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (isListening && HoldingKeyResult() && IsDelayDone())
            {
                if (!isWindowDetected)
                {
                    holdKeyUsed = true;

                    ActiveWindow = winApiService.GetActiveWindow();
                    ActiveWindow.Dpi = DpiHelper.GetDpiFromPoint(e.X, e.Y);

                    if (ActiveWindow?.Title != null && IsExcludedApplication(ActiveWindow.Title))
                    {
                        isListening = false;
                    }
                    else if (settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(ActiveWindow.Boundry))
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

                delayStartTime = DateTime.Now;
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtonMapper.Map(settingService.Settings.MouseButton) && isListening)
            {
                isListening = false;
                windowService.Hide();

                MoveActiveWindow(snapArea, e.Button == MouseButtons.Left);
            }
        }

        private void MoveActiveWindow(Rectangle rectangle, bool isLeftClick)
        {
            if (ActiveWindow != ActiveWindow.Empty)
            {
                if (!rectangle.Equals(Rectangle.Empty))
                {
                    winApiService.GetWindowMargin(ActiveWindow, out Rectangle withMargin);

                    if (!withMargin.Equals(default(Rectangle)))
                    {
                        var marginHorizontal = (ActiveWindow.Boundry.Width - withMargin.Width) / 2;
                        var systemMargin = new Rectangle
                        {
                            Left = marginHorizontal,
                            Right = marginHorizontal,
                            Top = 0,
                            Bottom = ActiveWindow.Boundry.Height - withMargin.Height
                        };

                        rectangle.Left -= systemMargin.Left;
                        rectangle.Top -= systemMargin.Top;
                        rectangle.Right += systemMargin.Right;
                        rectangle.Bottom += systemMargin.Bottom;
                    }

                    if (isLeftClick)
                    {
                        //winApiService.SendMessage(ActiveWindow);
                        //SendKeys.SendWait("{ESC}");

                        new Thread(() =>
                        {
                            Thread.Sleep(100);

                            winApiService.MoveWindow(ActiveWindow, rectangle);

                            if (!rectangle.Dpi.Equals(ActiveWindow.Dpi))
                            {
                                winApiService.MoveWindow(ActiveWindow, rectangle);
                            }
                        }).Start();
                    }
                    else
                    {
                        winApiService.MoveWindow(ActiveWindow, rectangle);

                        if (!rectangle.Dpi.Equals(ActiveWindow.Dpi))
                        {
                            winApiService.MoveWindow(ActiveWindow, rectangle);
                        }
                    }
                }
            }
        }
    }
}