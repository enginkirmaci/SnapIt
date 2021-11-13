using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using SnapIt.Library.Entities;
using Windows.System;

namespace SnapIt.Library.Services
{
    public class NotifyIconService : INotifyIconService
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private Window applicationWindow;
        private ToolStripLabel statusToolStrip;
        private ToolStripItem startToolStrip;
        private ToolStripItem stopToolStrip;
        private NotifyIcon notifyIcon;

        public event SetViewEvent SetView;

        public NotifyIconService(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
        }

        public void Initialize()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.DoubleClick += (s, args) => ShowDefaultWindow(ViewType.LayoutView);
            notifyIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Themes/notifyicon.ico")).Stream);
            notifyIcon.Visible = true;

            CreateContextMenu();
        }

        public void SetApplicationWindow(Window window)
        {
            applicationWindow = window;
        }

        public void Release()
        {
            notifyIcon.Dispose();
            notifyIcon = null;
        }

        private void CreateContextMenu()
        {
            statusToolStrip = new ToolStripLabel
            {
                Text = "Opening",
                Enabled = false
            };

            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add(statusToolStrip);
            notifyIcon.ContextMenuStrip.Items.Add("-");

            startToolStrip = notifyIcon.ContextMenuStrip.Items.Add("Start");
            stopToolStrip = notifyIcon.ContextMenuStrip.Items.Add("Stop");

            notifyIcon.ContextMenuStrip.Items.Add("-");

            var layoutsMenu = new ToolStripMenuItem("Layouts");
            layoutsMenu.Name = "LayoutsMenu";
            notifyIcon.ContextMenuStrip.Items.Add(layoutsMenu);

            notifyIcon.ContextMenuStrip.Items.Add("Settings").Click += (s, e) => ShowDefaultWindow(ViewType.SettingsView);

            notifyIcon.ContextMenuStrip.Items.Add("-");

            var feedbackMenu = new ToolStripMenuItem("Feedback");
            notifyIcon.ContextMenuStrip.Items.Add(feedbackMenu);
            feedbackMenu.DropDownItems.Add("Send new idea/bug").Click += (s, e) => Process.Start(new ProcessStartInfo
            {
                FileName = $"http://{Constants.AppFeedbackUrl}",
                UseShellExecute = true
            });

            feedbackMenu.DropDownItems.Add("Rate and review").Click += async (s, e) =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("Rate and review only works for Windows 10 or later versions");
                }
            };

            notifyIcon.ContextMenuStrip.Items.Add("About").Click += (s, e) => ShowDefaultWindow(ViewType.AboutView);

            notifyIcon.ContextMenuStrip.Items.Add("-");
            notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();

            startToolStrip.Click += StartToolStrip_Click;
            startToolStrip.Visible = false;
            stopToolStrip.Click += StopToolStrip_Click;
            stopToolStrip.Visible = false;

            snapService.StatusChanged += SnapService_StatusChanged;
            snapService.LayoutChanged += SnapService_LayoutChanged;
            snapService.ScreenLayoutLoaded += SnapService_ScreenLayoutLoaded;
        }

        private void SnapService_LayoutChanged(SnapScreen snapScreen, Layout layout)
        {
            ShowNotification("Layout changed", $"{layout.Name} layout is set to Display {snapScreen.DeviceNumber} ({snapScreen.Resolution})");
        }

        private void SnapService_ScreenLayoutLoaded(IList<SnapScreen> snapScreens, IList<Layout> layouts)
        {
            var layoutsMenu = notifyIcon.ContextMenuStrip.Items["LayoutsMenu"] as ToolStripMenuItem;
            layoutsMenu.DropDownItems.Clear();

            foreach (var screen in snapScreens)
            {
                var screenMenu = new ToolStripMenuItem($"Display {screen.DeviceNumber} ({screen.Resolution}) - {screen.Primary}");
                screenMenu.Name = screen.Base.DeviceName;

                if (snapScreens.Count > 1)
                {
                    layoutsMenu.DropDownItems.Add(screenMenu);
                }

                foreach (var layout in layouts)
                {
                    var layoutMenuItem = new ToolStripMenuItem(layout.Name)
                    {
                        Tag = screen.Base.DeviceName,
                        Name = layout.Guid.ToString(),
                        CheckOnClick = true
                    };
                    layoutMenuItem.Click += LayoutItem_Click;

                    if (screen.Layout == layout)
                    {
                        layoutMenuItem.Checked = true;
                    }

                    if (snapScreens.Count > 1)
                    {
                        screenMenu.DropDownItems.Add(layoutMenuItem);
                    }
                    else
                    {
                        layoutsMenu.DropDownItems.Add(layoutMenuItem);
                    }
                }
            }
        }

        private void LayoutItem_Click(object sender, EventArgs e)
        {
            var layoutMenuItem = sender as ToolStripMenuItem;

            var selectedSnapScreen = settingService.SnapScreens.FirstOrDefault();

            if (settingService.SnapScreens.Count > 1)
            {
                var snapScreen = settingService.SnapScreens.FirstOrDefault(screen => screen.Base.DeviceName == layoutMenuItem.Tag.ToString());

                if (snapScreen != null)
                {
                    selectedSnapScreen = snapScreen;
                }
            }

            var selectedLayout = settingService.Layouts.FirstOrDefault(layout => layout.Guid.ToString() == layoutMenuItem.Name);

            SnapService_LayoutChanged(selectedSnapScreen, selectedLayout);

            settingService.LinkScreenLayout(selectedSnapScreen, selectedLayout);
            snapService.Release();
            snapService.Initialize();
        }

        public void ShowNotification(string title, string message, int timeout = 1000, ToolTipIcon tipIcon = ToolTipIcon.None)
        {
            notifyIcon.ShowBalloonTip(timeout, title, message, tipIcon);
        }

        private void SnapService_StatusChanged(bool isRunning)
        {
            if (isRunning)
            {
                statusToolStrip.Text = "Running";
                startToolStrip.Visible = false;
                stopToolStrip.Visible = true;
            }
            else
            {
                statusToolStrip.Text = "Stopped";
                startToolStrip.Visible = true;
                stopToolStrip.Visible = false;
            }
        }

        private void StartToolStrip_Click(object sender, EventArgs e)
        {
            snapService.Initialize();
        }

        private void StopToolStrip_Click(object sender, EventArgs e)
        {
            snapService.Release();
        }

        private void ExitApplication()
        {
            applicationWindow.Close();

            System.Windows.Application.Current.Shutdown();
        }

        private void ShowDefaultWindow(ViewType viewType)
        {
            if (applicationWindow.IsVisible)
            {
                if (applicationWindow.WindowState == WindowState.Minimized)
                {
                    applicationWindow.WindowState = WindowState.Normal;
                }

                applicationWindow.Activate();
            }
            else
            {
                applicationWindow.Show();

                applicationWindow.Activate();
            }

            SetView?.Invoke(viewType);
        }
    }
}