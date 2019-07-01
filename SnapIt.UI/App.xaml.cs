using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Prism.Ioc;
using SnapIt.Configuration;
using SnapIt.Services;
using SnapIt.UI.Views;

namespace SnapIt.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static Mutex mutex = new Mutex(true, "{FE4F369C-450C-4FA5-ACCA-3D261A3A7969}");

        private NotifyIcon _notifyIcon;
        private bool _isExit;

        private Window DefaultWindow { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                mutex.ReleaseMutex();
            }
            else
            {
                System.Windows.MessageBox.Show("only one instance at a time");

                Shutdown();
                return;
            }

            base.OnStartup(e);

            _notifyIcon = new NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowDefaultWindow();
            _notifyIcon.Icon = new System.Drawing.Icon(GetResourceStream(new Uri("pack://application:,,,/Themes/app.ico")).Stream);
            _notifyIcon.Visible = true;

            CreateContextMenu();
        }

        protected override Window CreateShell()
        {
            DefaultWindow = Container.Resolve<MainWindow>();
            return DefaultWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConfigService, ConfigService>();
            containerRegistry.RegisterSingleton<ISnapService, SnapService>();
            containerRegistry.Register<IWindowService, WindowService>();
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("MainWindow...").Click += (s, e) => ShowDefaultWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            DefaultWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;

            Shutdown();
        }

        private void ShowDefaultWindow()
        {
            if (DefaultWindow.IsVisible)
            {
                if (DefaultWindow.WindowState == WindowState.Minimized)
                {
                    DefaultWindow.WindowState = WindowState.Normal;
                }
                DefaultWindow.Activate();
            }
            else
            {
                DefaultWindow.Show();
            }
        }

        private void DefaultWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                DefaultWindow.Hide();
            }
        }
    }
}