using System;
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

        private NotifyIcon notifyIcon;

        private Window ApplicationWindow { get; set; }

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

            notifyIcon = new NotifyIcon();
            notifyIcon.DoubleClick += (s, args) => ShowDefaultWindow();
            notifyIcon.Icon = new System.Drawing.Icon(GetResourceStream(new Uri("pack://application:,,,/Themes/app.ico")).Stream);
            notifyIcon.Visible = true;

            CreateContextMenu();
        }

        protected override Window CreateShell()
        {
            ApplicationWindow = Container.Resolve<MainWindow>();
            return ApplicationWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IConfigService, ConfigService>();
            containerRegistry.RegisterSingleton<ISnapService, SnapService>();
            containerRegistry.Register<IWindowService, WindowService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            notifyIcon.Dispose();
            notifyIcon = null;

            Container.Resolve<ISnapService>().Release();
        }

        private void CreateContextMenu()
        {
            notifyIcon.ContextMenuStrip = new ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("MainWindow...").Click += (s, e) => ShowDefaultWindow();
            notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            ApplicationWindow.Close();

            Shutdown();
        }

        private void ShowDefaultWindow()
        {
            if (ApplicationWindow.IsVisible)
            {
                if (ApplicationWindow.WindowState == WindowState.Minimized)
                {
                    ApplicationWindow.WindowState = WindowState.Normal;
                }

                ApplicationWindow.Activate();
            }
            else
            {
                ApplicationWindow.Show();
            }
        }
    }
}