using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using DryIoc;
using Prism.Ioc;
using Serilog;
using SnapIt.Library;
using SnapIt.Library.Applications;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static NotifyIcon NotifyIcon { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
                       .CreateLogger();
            RegisterGlobalExceptionHandling(Log.Logger);

            //todo change this
            NotifyIcon = new NotifyIcon
            {
                Icon = new Icon(GetResourceStream(new Uri("pack://application:,,,/Themes/notifyicon.ico")).Stream)
            };

#if STANDALONE
            if (SnapIt.Properties.Settings.Default.RunAsAdmin && !DevMode.IsActive)
            {
                if (e.Args.Length > 0 && RunAsAdministrator.IsAdmin(e.Args))
                {
                    if (!ApplicationInstance.RegisterSingleInstance())
                    {
                        NotifyIcon.ShowBalloonTip(3000, null, $"Only one instance of {Constants.AppName} can run at the same time.", ToolTipIcon.Warning);
                        NotifyIcon.Visible = true;

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
                    NotifyIcon.ShowBalloonTip(3000, null, $"Only one instance of {Constants.AppName} can run at the same time.", ToolTipIcon.Warning);
                    NotifyIcon.Visible = true;

                    Shutdown();
                    return;
                }
            }

            //AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            //{
            //    LogUnhandledException((Exception)ex.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            //};

            //DispatcherUnhandledException += (s, ex) =>
            //{
            //    LogUnhandledException(ex.Exception,
            //    "Application.Current.DispatcherUnhandledException");
            //    ex.Handled = true;
            //};

            //TaskScheduler.UnobservedTaskException += (s, ex) =>
            //{
            //    LogUnhandledException(ex.Exception,
            //    "TaskScheduler.UnobservedTaskException");
            //    ex.SetObserved();
            //};

            Telemetry.TrackEvent("OnStartup");
            Log.Logger.Information("SnapIt Started");

            base.OnStartup(e);
        }

        //private void LogUnhandledException(Exception e, string @event)
        //{
        //    Telemetry.TrackException(e);
        //}

        private void RegisterGlobalExceptionHandling(ILogger log)
        {
            // this is the line you really want
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => CurrentDomainOnUnhandledException(args, log);

            // optional: hooking up some more handlers
            // remember that you need to hook up additional handlers when
            // logging from other dispatchers, shedulers, or applications
            DispatcherUnhandledException += (sender, args) => CurrentOnDispatcherUnhandledException(args, log);

            Dispatcher.UnhandledException += (sender, args) => DispatcherOnUnhandledException(args, log);

            TaskScheduler.UnobservedTaskException +=
                (sender, args) => TaskSchedulerOnUnobservedTaskException(args, log);
        }

        private static void CurrentDomainOnUnhandledException(UnhandledExceptionEventArgs args, ILogger log)
        {
            var exception = args.ExceptionObject as Exception;
            var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
            var exceptionMessage = exception?.Message ?? "An unmanaged exception occured.";
            var message = string.Concat(exceptionMessage, terminatingMessage);
            log.Error(exception, message);
        }

        private static void CurrentOnDispatcherUnhandledException(DispatcherUnhandledExceptionEventArgs args, ILogger log)
        {
            log.Error(args.Exception, args.Exception.Message);
            args.Handled = true;
        }

        private static void DispatcherOnUnhandledException(DispatcherUnhandledExceptionEventArgs args, ILogger log)
        {
            log.Error(args.Exception, args.Exception.Message);
            args.Handled = true;
        }

        private static void TaskSchedulerOnUnobservedTaskException(UnobservedTaskExceptionEventArgs args, ILogger log)
        {
            log.Error(args.Exception, args.Exception.Message);
            args.SetObserved();
        }

        protected override Window CreateShell()
        {
            var applicationWindow = Container.Resolve<MainWindow>();

            return applicationWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<HomeView>();
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
                    NotifyIcon.Dispose();
                }
            }

            Log.Logger.Information("SnapIt Exited");
        }
    }
}