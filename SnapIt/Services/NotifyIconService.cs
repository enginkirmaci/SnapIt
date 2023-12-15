using System.Windows;
using Prism.Commands;
using SnapIt.Application.Contracts;
using SnapIt.Common.Contracts;
using SnapIt.Common.Converters;
using SnapIt.Common.Entities;
using SnapIt.Services.Contracts;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace SnapIt.Services;

public interface INotifyIconService : IInitialize
{
}

public class NotifyIconService : Wpf.Ui.Tray.NotifyIconService, INotifyIconService
{
    private readonly ISnapManager snapManager;
    private readonly ISettingService settingService;
    private MenuItem statusItem;
    private MenuItem startItemMenu;
    private MenuItem stopItemMenu;
    private MenuItem layoutItem;

    public bool IsInitialized { get; private set; }

    public DelegateCommand StartStopCommand { get; private set; }
    public DelegateCommand<string> NavigateClickViewCommand { get; private set; }
    public DelegateCommand<string> HandleLinkCommand { get; private set; }
    public DelegateCommand RateReviewStoreCommand { get; private set; }
    public DelegateCommand ExitApplicationCommand { get; private set; }

    public NotifyIconService(
        ISnapManager snapManager,
        INavigationService navigationService,
        ISettingService settingService)
    {
        this.snapManager = snapManager;
        this.settingService = settingService;
        StartStopCommand = new DelegateCommand(snapManager.StartStop);

        NavigateClickViewCommand = new DelegateCommand<string>((parameter) =>
        {
            var mainWindow = System.Windows.Application.Current.MainWindow;
            if (mainWindow != null && mainWindow.IsVisible)
            {
                if (mainWindow.WindowState == WindowState.Minimized)
                {
                    mainWindow.WindowState = WindowState.Normal;
                }

                mainWindow.Activate();
            }
            else
            {
                mainWindow.Show();
                mainWindow.Activate();
            }

            navigationService.Navigate(NameToPageTypeConverter.Convert(parameter));
        });

        HandleLinkCommand = new DelegateCommand<string>((url) =>
        {
            string uriToLaunch = $"https://{url}";
            Process.Start(new ProcessStartInfo
            {
                FileName = uriToLaunch,
                UseShellExecute = true
            });
        });

        RateReviewStoreCommand = new DelegateCommand(async () =>
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Rate and review only works for Windows 10 or later versions");
            }
        });

        ExitApplicationCommand = new DelegateCommand(() =>
        {
            System.Windows.Application.Current.MainWindow.Close();

            System.Windows.Application.Current.Shutdown();
        });
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        TooltipText = Constants.AppName;
        ContextMenu = new System.Windows.Controls.ContextMenu();
        Register();

        statusItem = new MenuItem { Header = string.Empty, IsEnabled = false };
        ContextMenu.Items.Add(statusItem);

        startItemMenu = new MenuItem
        {
            Header = "Start",
            Icon = new SymbolIcon(SymbolRegular.Play24),
            Command = StartStopCommand
        };
        ContextMenu.Items.Add(startItemMenu);

        stopItemMenu = new MenuItem
        {
            Header = "Stop",
            Icon = new SymbolIcon(SymbolRegular.Stop24),
            Command = StartStopCommand
        };
        ContextMenu.Items.Add(stopItemMenu);

        ContextMenu.Items.Add(new System.Windows.Controls.Separator());

        ContextMenu.Items.Add(new MenuItem
        {
            Header = "Home",
            Icon = new SymbolIcon(SymbolRegular.Home24),
            Command = NavigateClickViewCommand,
            CommandParameter = "dashboard"
        });

        ContextMenu.Items.Add(new System.Windows.Controls.Separator());

        layoutItem = new MenuItem
        {
            Header = "Layouts",
            Icon = new SymbolIcon(SymbolRegular.DataTreemap24),
            Command = NavigateClickViewCommand,
            CommandParameter = "layout"
        };
        ContextMenu.Items.Add(layoutItem);

        ContextMenu.Items.Add(new MenuItem
        {
            Header = "Settings",
            Icon = new SymbolIcon(SymbolRegular.Settings24),
            Command = NavigateClickViewCommand,
            CommandParameter = "settings"
        });

        ContextMenu.Items.Add(new System.Windows.Controls.Separator());

        var feedback = new MenuItem
        {
            Header = "Feedback",
            Icon = new SymbolIcon(SymbolRegular.Heart24)
        };
        feedback.Items.Add(new MenuItem
        {
            Header = "New ideas or report a bug",
            Command = HandleLinkCommand,
            CommandParameter = Constants.AppFeedbackUrl
        });
        feedback.Items.Add(new MenuItem
        {
            Header = "Rate and review on Microsoft Store",
            Command = RateReviewStoreCommand
        });
        ContextMenu.Items.Add(feedback);

        ContextMenu.Items.Add(new MenuItem
        {
            Header = "About",
            Icon = new SymbolIcon(SymbolRegular.Info24),
            Command = NavigateClickViewCommand,
            CommandParameter = "about"
        });

        ContextMenu.Items.Add(new System.Windows.Controls.Separator());

        ContextMenu.Items.Add(new MenuItem
        {
            Header = "Exit",
            Icon = new SymbolIcon(SymbolRegular.Power24),
            Command = ExitApplicationCommand
        });

        IsInitialized = true;
    }

    protected override void OnRightClick()
    {
        base.OnRightClick();

        statusItem.Header = snapManager.IsRunning ? "Running" : "Stopped";

        if (!snapManager.IsRunning)
        {
            startItemMenu.Visibility = Visibility.Visible;
            stopItemMenu.Visibility = Visibility.Collapsed;
        }
        else
        {
            startItemMenu.Visibility = Visibility.Collapsed;
            stopItemMenu.Visibility = Visibility.Visible;
        }

        layoutItem.Items.Clear();

        var snapScreens = settingService.SnapScreens;
        var layouts = settingService.Layouts;

        foreach (var screen in snapScreens)
        {
            var screenMenu = new MenuItem()
            {
                Header = $"Display {screen.DeviceNumber} ({screen.Resolution}) - {screen.Primary}"
            };

            if (snapScreens.Count > 1)
            {
                layoutItem.Items.Add(screenMenu);
            }

            foreach (var layout in layouts)
            {
                var layoutMenuItem = new MenuItem()
                {
                    Header = layout.Name,
                    Tag = screen.DeviceName,
                    Uid = layout.Guid.ToString(),
                    Icon = new SymbolIcon(SymbolRegular.Empty)
                };
                layoutMenuItem.Click += LayoutItem_Click;

                if (screen.Layout == layout)
                {
                    layoutMenuItem.Icon = new SymbolIcon(SymbolRegular.Checkmark24);
                }
                else
                {
                    layoutMenuItem.Header = "     " + layout.Name;
                }

                if (snapScreens.Count > 1)
                {
                    screenMenu.Items.Add(layoutMenuItem);
                }
                else
                {
                    layoutItem.Items.Add(layoutMenuItem);
                }
            }
        }
    }

    public void Dispose()
    {
        IsInitialized = false;
    }

    private void LayoutItem_Click(object sender, EventArgs e)
    {
        var layoutMenuItem = sender as MenuItem;

        var selectedSnapScreen = settingService.SnapScreens.FirstOrDefault();

        if (settingService.SnapScreens.Count > 1)
        {
            var snapScreen = settingService.SnapScreens.FirstOrDefault(screen => screen.DeviceName == layoutMenuItem.Tag.ToString());

            if (snapScreen != null)
            {
                selectedSnapScreen = snapScreen;
            }
        }

        var selectedLayout = settingService.Layouts.FirstOrDefault(layout => layout.Guid.ToString() == layoutMenuItem.Uid);

        //SnapService_LayoutChanged(selectedSnapScreen, selectedLayout);

        settingService.LinkScreenLayout(selectedSnapScreen, selectedLayout);
        snapManager.Dispose();
        snapManager.InitializeAsync();
    }
}