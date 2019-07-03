using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace SnapIt.Services
{
    public class NotifyIconService : INotifyIconService
    {
        private readonly ISnapService snapService;

        private Window applicationWindow;
        private NotifyIcon notifyIcon;
        private ToolStripLabel statusToolStrip;
        private ToolStripItem startToolStrip;
        private ToolStripItem stopToolStrip;

        public NotifyIconService(ISnapService snapService)
        {
            this.snapService = snapService;
        }

        public void Initialize()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.DoubleClick += (s, args) => ShowDefaultWindow();
            notifyIcon.Icon = new Icon(System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Themes/app.ico")).Stream);
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
            notifyIcon.ContextMenuStrip.Items.Add("Settings").Click += (s, e) => ShowDefaultWindow();
            notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();

            startToolStrip.Click += StartToolStrip_Click;
            startToolStrip.Visible = false;
            stopToolStrip.Click += StopToolStrip_Click;
            stopToolStrip.Visible = false;

            snapService.StatusChanged += SnapService_StatusChanged;
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

        private void ShowDefaultWindow()
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
            }
        }
    }
}