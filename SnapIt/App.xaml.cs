﻿using System.IO;
using System.Windows;
using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Serilog;
using SnapIt.Application;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Applications;
using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Services;
using SnapIt.Services.Contracts;
using SnapIt.Views.Windows;
using Wpf.Ui;

namespace SnapIt;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public static IContainer AppContainer { get; set; }

    //public static NotifyIcon NotifyIcon { get; set; }
    public static Assembly Assembly => Assembly.GetExecutingAssembly();

    public string[] startupArgs;

    protected override Window CreateShell()
    {
        //if (!Dev.IsActive)
        //{
        //    var snapManager = AppContainer.Resolve<ISnapManager>();
        //    _ = snapManager.InitializeAsync();
        //}
        var settingService = AppContainer.Resolve<ISettingService>();

        settingService.LoadSettings();

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

        Telemetry.TrackEvent("OnStartup");
        Log.Logger.Information("SnapIt Started");

        var applicationWindow = AppContainer.Resolve<MainWindow>();

        return applicationWindow;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IWindow, MainWindow>();

        containerRegistry.AddTransientFromNamespace("SnapIt.Views", Assembly);
        containerRegistry.AddTransientFromNamespace("SnapIt.ViewModels", Assembly);

        containerRegistry.RegisterSingleton<ISnapManager, SnapManager>();
        containerRegistry.RegisterSingleton<IWindowManager, WindowManager>();
        containerRegistry.RegisterSingleton<IScreenManager, ScreenManager>();

        containerRegistry.RegisterSingleton<IGlobalHookService, GlobalHookService>();
        containerRegistry.RegisterSingleton<IMouseService, MouseService>();
        containerRegistry.RegisterSingleton<IKeyboardService, KeyboardService>();
        containerRegistry.RegisterSingleton<IFileOperationService, FileOperationService>();
        containerRegistry.RegisterSingleton<ISettingService, SettingService>();
        containerRegistry.RegisterSingleton<IHotkeyService, HotkeyService>();
        containerRegistry.RegisterSingleton<IWinApiService, WinApiService>();
        containerRegistry.RegisterSingleton<IStoreLicenseService, StoreLicenseService>();
        containerRegistry.RegisterSingleton<IWindowsService, WindowsService>();

        containerRegistry.RegisterSingleton<IThemeService, ThemeService>();
        containerRegistry.RegisterSingleton<INavigationService, NavigationService>();
        containerRegistry.RegisterSingleton<ISnackbarService, SnackbarService>();
        containerRegistry.RegisterSingleton<IContentDialogService, ContentDialogService>();
        containerRegistry.RegisterSingleton<INotifyIconService, NotifyIconService>();

        AppContainer = containerRegistry.GetContainer();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.File(Path.Combine(Constants.RootFolder, "logs", "log.txt"),
                        rollingInterval: RollingInterval.Day)
                   .CreateLogger();

        RegisterGlobalExceptionHandling(Log.Logger);

        //todo change this
        //NotifyIcon = new NotifyIcon
        //{
        //    Icon = new System.Drawing.Icon(GetResourceStream(new Uri("pack://application:,,,/Assets/app.ico")).Stream)
        //};

        startupArgs = e.Args;
    }

    private void OnExit(object sender, ExitEventArgs e)
    {
        if (AppContainer != null)
        {
            var snapServiceContainer = AppContainer.Resolve<ISnapManager>();

            snapServiceContainer?.Dispose();

            var globalHookService = AppContainer.Resolve<IGlobalHookService>();
            globalHookService?.Hook?.Dispose();
        }

        Log.Logger.Information("SnapIt Exited");
    }

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