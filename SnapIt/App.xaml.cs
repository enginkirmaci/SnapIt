using System.IO;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SnapIt.Application;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Applications;
using SnapIt.Common.Extensions;
using SnapIt.Services;
using SnapIt.Services.Contracts;
using SnapIt.ViewModels.Windows;
using SnapIt.Views.Windows;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace SnapIt;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public string[] startupArgs;
    public static Assembly Assembly => Assembly.GetExecutingAssembly();

    // Configure Serilog logger before host creation
    static App()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(AppContext.BaseDirectory, "logs", "log.txt"),
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            //.WriteTo.Console()
            .CreateLogger();
    }

    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .UseSerilog() // <-- Integrate Serilog with the host
        .ConfigureAppConfiguration(c =>
        {
            var basePath =
                Path.GetDirectoryName(AppContext.BaseDirectory)
                ?? throw new DirectoryNotFoundException(
                    "Unable to find the base directory of the application."
                );
            _ = c.SetBasePath(basePath);
        })
        .ConfigureServices(
            (context, services) =>
            {
                _ = services.AddNavigationViewPageProvider();

                // App Host
                _ = services.AddHostedService<ApplicationHostService>();

                _ = services.AddSingleton<ISnackbarService, SnackbarService>();
                _ = services.AddSingleton<IContentDialogService, ContentDialogService>();
                _ = services.AddSingleton<INotifyIconService, NotifyIconService>();
                _ = services.AddSingleton<IThemeService, ThemeService>();
                _ = services.AddSingleton<ITaskBarService, TaskBarService>();
                _ = services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                _ = services.AddSingleton<INavigationWindow, MainWindow>();
                _ = services.AddSingleton<MainWindowViewModel>();

                // Views and ViewModels
                _ = services.AddTransientFromNamespace("SnapIt.Views", Assembly);
                _ = services.AddTransientFromNamespace("SnapIt.ViewModels", Assembly);

                _ = services.AddSingleton<ISnapManager, SnapManager>();
                _ = services.AddSingleton<IWindowManager, WindowManager>();
                _ = services.AddSingleton<IScreenManager, ScreenManager>();

                _ = services.AddSingleton<IGlobalHookService, GlobalHookService>();
                _ = services.AddSingleton<IMouseService, MouseService>();
                _ = services.AddSingleton<IKeyboardService, KeyboardService>();
                _ = services.AddSingleton<IFileOperationService, DatabaseOperationService>();
                _ = services.AddSingleton<ISettingService, SettingService>();
                _ = services.AddSingleton<IHotkeyService, HotkeyService>();
                _ = services.AddSingleton<IWinApiService, WinApiService>();
                _ = services.AddSingleton<IStoreLicenseService, StoreLicenseService>();
                _ = services.AddSingleton<IWindowsService, WindowsService>();
                _ = services.AddSingleton<IWindowEventService, WindowEventService>();
            }
        )
        .Build();

    /// <summary>
    /// Gets services.
    /// </summary>
    public static IServiceProvider Services
    {
        get { return _host.Services; }
    }

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        startupArgs = e.Args;
        Telemetry.TrackEvent("OnStartup");
        Log.Logger.Information("SnapIt Started");
        RegisterGlobalExceptionHandling(Log.Logger);

        await _host.StartAsync();

        var settingService = Services.GetRequiredService<ISettingService>();

        await settingService.LoadSettingsAsync();

        if (!AppLauncher.BypassSingleInstance(startupArgs))
        {
            if (settingService.Settings.RunAsAdmin && !Dev.SkipRunAsAdmin)
            {
                if (AppLauncher.IsAdmin(startupArgs))
                {
                    if (!AppInstance.RegisterSingleInstance())
                    {
                        //NotifyIcon.ShowBalloonTip(3000, null, $"Only one instance of {Constants.AppName} can run at the same time.", ToolTipIcon.Warning);
                        //NotifyIcon.Visible = true;

                        Shutdown();
                    }
                }
                else if (!Dev.IsActive)
                {
                    AppLauncher.RunAsAdmin();
                    Shutdown();
                }
            }
            else
            {
                if (!AppInstance.RegisterSingleInstance() && !Dev.IsActive)
                {
                    //NotifyIcon.ShowBalloonTip(3000, null, $"Only one instance of {Constants.AppName} can run at the same time.", ToolTipIcon.Warning);
                    //NotifyIcon.Visible = true;

                    Shutdown();
                }
            }
        }
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();

        Log.Logger.Information("SnapIt Exited");
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
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

        TaskScheduler.UnobservedTaskException += (sender, args) => TaskSchedulerOnUnobservedTaskException(args, log);
    }

    private static void CurrentDomainOnUnhandledException(UnhandledExceptionEventArgs args, ILogger log)
    {
        //AppLauncher.RunBypassSingleInstance();

        var exception = args.ExceptionObject as Exception;
        var terminatingMessage = args.IsTerminating ? " The application is terminating." : string.Empty;
        var exceptionMessage = exception?.Message ?? "An unmanaged exception occured.";
        var message = string.Concat(exceptionMessage, terminatingMessage, exception?.StackTrace, exception?.InnerException);
        log.Error(exception, message);

        if (exception?.InnerException != null)
        {
            log.Error(exception?.InnerException, message);
        }
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
}
