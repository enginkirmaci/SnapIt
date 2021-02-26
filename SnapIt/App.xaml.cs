using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using DryIoc;
using Prism.Ioc;
using Prism.Regions;
using SnapIt.Library;
using SnapIt.Library.Applications;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //if (SnapIt.Properties.Settings.Default.RunAsAdmin && !DevMode.IsActive)
            //{
            //	if (e.Args.Length > 0 && RunAsAdministrator.IsAdmin(e.Args))
            //	{
            //		if (!ApplicationInstance.RegisterSingleInstance())
            //		{
            //			MessageBox.Show("only one instance at a time");

            //			Shutdown();
            //			return;
            //		}
            //	}
            //	else
            //	{
            //		RunAsAdministrator.Run();
            //		Shutdown();
            //		return;
            //	}
            //}
            //else

            //{
            if (!ApplicationInstance.RegisterSingleInstance() && !DevMode.IsActive)
            {
                var notifyIcon = new System.Windows.Forms.NotifyIcon
                {
                    Icon = new Icon(GetResourceStream(new Uri("pack://application:,,,/Themes/notifyicon.ico")).Stream),
                    Visible = true
                };
                notifyIcon.ShowBalloonTip(3000, null, "Only one instance of Snap It can run at the same time.", System.Windows.Forms.ToolTipIcon.Warning);

                Shutdown();
                return;
            }
            //}

            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                LogUnhandledException((Exception)ex.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                LogUnhandledException(ex.Exception,
                "Application.Current.DispatcherUnhandledException");
                ex.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, ex) =>
            {
                LogUnhandledException(ex.Exception,
                "TaskScheduler.UnobservedTaskException");
                ex.SetObserved();
            };

            Telemetry.TrackEvent("OnStartup");

            base.OnStartup(e);
        }

        private void LogUnhandledException(Exception e, string @event)
        {
            Telemetry.TrackException(e);
        }

        protected override Window CreateShell()
        {
            var notifyIconService = Container.Resolve<INotifyIconService>();
            var regionManager = Container.Resolve<IRegionManager>();

            notifyIconService.Initialize();

            var applicationWindow = Container.Resolve<MainWindow>();
            applicationWindow.Initialize(notifyIconService, regionManager);
            notifyIconService.SetApplicationWindow(applicationWindow);

            return applicationWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LayoutView>();
            containerRegistry.RegisterForNavigation<MouseSettingsView>();
            containerRegistry.RegisterForNavigation<KeyboardSettingsView>();
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.RegisterForNavigation<WindowsView>();
            containerRegistry.RegisterForNavigation<ThemeView>();
            containerRegistry.RegisterForNavigation<WhatsNewView>();
            containerRegistry.RegisterForNavigation<AboutView>();

            containerRegistry.RegisterSingleton<INotifyIconService, NotifyIconService>();
            containerRegistry.RegisterSingleton<IFileOperationService, FileOperationService>();
            containerRegistry.RegisterSingleton<ISnapService, SnapService>();
            containerRegistry.RegisterSingleton<ISettingService, SettingService>();
            containerRegistry.RegisterSingleton<IWinApiService, WinApiService>();
            containerRegistry.Register<IWindowService, WindowService>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (Container != null)
            {
                Container.Resolve<INotifyIconService>().Release();
                Container.Resolve<ISnapService>().Release();
            }
        }
    }
}