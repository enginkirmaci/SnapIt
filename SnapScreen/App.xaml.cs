using System;
using System.Threading.Tasks;
using System.Windows;
using DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using SnapScreen.Library;
using SnapScreen.Library.Applications;
using SnapScreen.Library.Entities;
using SnapScreen.Library.Services;
using SnapScreen.Views;

namespace SnapScreen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
#if STANDALONE
            if (SnapScreen.Properties.Settings.Default.RunAsAdmin && !DevMode.IsActive)
            {
                if (e.Args.Length > 0 && RunAsAdministrator.IsAdmin(e.Args))
                {
                    if (!ApplicationInstance.RegisterSingleInstance())
                    {
                        MessageBox.Show("only one instance at a time");

                        Shutdown();
                        return;
                    }
                }
                else if (!DevMode.IsActive)
                {
                    RunAsAdministrator.Run();
                    Shutdown();
                    return;
                }
            }
            else
#endif
            {
                if (!ApplicationInstance.RegisterSingleInstance() && !DevMode.IsActive)
                {
                    var notification = new System.Windows.Forms.NotifyIcon
                    {
                        Visible = true,
                        Icon = new System.Drawing.Icon(GetResourceStream(new Uri("pack://application:,,,/Themes/notifyicon.ico")).Stream),
                    };

                    notification.ShowBalloonTip(3000, null, $"Only one instance of {Constants.AppName} can run at the same time.", System.Windows.Forms.ToolTipIcon.Warning);

                    Shutdown();
                    return;
                }
            }

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
            var applicationWindow = Container.Resolve<MainWindow>();

            return applicationWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LayoutView>();
            containerRegistry.RegisterForNavigation<MouseSettingsView>();
            containerRegistry.RegisterForNavigation<KeyboardSettingsView>();
            containerRegistry.RegisterForNavigation<WindowsView>();
            containerRegistry.RegisterForNavigation<ThemeView>();
            containerRegistry.RegisterForNavigation<SettingsView>();
            containerRegistry.RegisterForNavigation<WhatsNewView>();
            containerRegistry.RegisterForNavigation<AboutView>();

            containerRegistry.RegisterSingleton<IFileOperationService, FileOperationService>();
            containerRegistry.RegisterSingleton<ISettingService, SettingService>();
            containerRegistry.RegisterSingleton<ISnapService, SnapService>();
            containerRegistry.RegisterSingleton<IWinApiService, WinApiService>();
            containerRegistry.Register<IWindowService, WindowService>();
            containerRegistry.RegisterSingleton<IStandaloneLicenseService, StandaloneLicenseService>();
            containerRegistry.RegisterSingleton<IStoreLicenseService, StoreLicenseService>();
            containerRegistry.RegisterSingleton<IScreenChangeService, ScreenChangeService>();

            ViewModelLocationProvider.Register<SettingsView, SnapIt.ViewModels.SettingsViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (Container != null)
            {
                var snapServiceContainer = Container.Resolve<ISnapService>();
                if (snapServiceContainer != null)
                {
                    snapServiceContainer.Release();
                }
            }
        }
    }
}