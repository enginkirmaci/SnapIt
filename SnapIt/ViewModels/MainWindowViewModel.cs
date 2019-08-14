using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Resources;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private bool HideWindowAtStartup = true;
        private ObservableCollection<MouseButton> mouseButtons;
        private ObservableCollection<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private ObservableCollection<Layout> layouts;
        private Layout selectedLayout;
        private bool isStartupTaskActive;
        private ObservableCollection<string> runningApplications;
        private string selectedApplication;
        private string selectedExcludedApplication;
        private ObservableCollection<string> excludedApplications;
        private bool isRenameDialogOpen;
        private Layout renameLayout;

        public string Title { get; set; } = $"{Constants.AppName} {System.Windows.Forms.Application.ProductVersion}";
        public bool EnableKeyboard { get => settingService.Settings.EnableKeyboard; set { settingService.Settings.EnableKeyboard = value; ApplyChanges(); } }
        public bool EnableMouse { get => settingService.Settings.EnableMouse; set { settingService.Settings.EnableMouse = value; ApplyChanges(); } }
        public bool DragByTitle { get => settingService.Settings.DragByTitle; set { settingService.Settings.DragByTitle = value; ApplyChanges(); } }
        public MouseButton MouseButton { get => settingService.Settings.MouseButton; set { settingService.Settings.MouseButton = value; ApplyChanges(); } }
        public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
        public bool DisableForFullscreen { get => settingService.Settings.DisableForFullscreen; set { settingService.Settings.DisableForFullscreen = value; ApplyChanges(); } }
        public bool IsStartupTaskActive
        {
            get => isStartupTaskActive;
            set
            {
                SetProperty(ref isStartupTaskActive, value);
                settingService.SetStartupTaskStatusAsync(value);
            }
        }

        public bool IsRenameDialogOpen { get => isRenameDialogOpen; set => SetProperty(ref isRenameDialogOpen, value); }
        public Layout RenameLayout { get => renameLayout; set => SetProperty(ref renameLayout, value); }

        //public bool IsRunAsAdmin
        //{
        //	get => Properties.Settings.Default.RunAsAdmin;
        //	set
        //	{
        //		Properties.Settings.Default.RunAsAdmin = value;
        //		Properties.Settings.Default.Save();
        //	}
        //}

        public ObservableCollection<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }
        public SnapScreen SelectedSnapScreen
        {
            get => selectedSnapScreen;
            set
            {
                SetProperty(ref selectedSnapScreen, value);
                SelectedLayout = selectedSnapScreen.Layout;
            }
        }

        public ObservableCollection<Layout> Layouts { get => layouts; set => SetProperty(ref layouts, value); }

        public Layout SelectedLayout
        {
            get => selectedLayout;
            set
            {
                if (value != null)
                {
                    SetProperty(ref selectedLayout, value);
                    //SaveLayoutCommand.RaiseCanExecuteChanged();

                    SelectedSnapScreen.Layout = selectedLayout;
                    settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);
                    ApplyChanges();
                }
            }
        }

        public ObservableCollection<string> RunningApplications { get => runningApplications; set => SetProperty(ref runningApplications, value); }
        public string SelectedApplication { get => selectedApplication; set => SetProperty(ref selectedApplication, value); }
        public ObservableCollection<string> ExcludedApplications { get => excludedApplications; set => SetProperty(ref excludedApplications, value); }
        public string SelectedExcludedApplication { get => selectedExcludedApplication; set => SetProperty(ref selectedExcludedApplication, value); }

        public DelegateCommand<Window> ActivatedCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand NewLayoutCommand { get; private set; }
        public DelegateCommand DesignLayoutCommand { get; private set; }
        public DelegateCommand ExportLayoutCommand { get; private set; }
        public DelegateCommand ImportLayoutCommand { get; private set; }
        public DelegateCommand<Layout> OpenRenameDialogCommand { get; private set; }

        public DelegateCommand ExcludeAppLayoutCommand { get; private set; }
        public DelegateCommand IncludeAppLayoutCommand { get; private set; }

        public DelegateCommand<string> HandleLinkClick { get; private set; }

        public MainWindowViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            Layouts = new ObservableCollection<Layout>(settingService.Layouts);
            SnapScreens = new ObservableCollection<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens.FirstOrDefault();
            RunningApplications = new ObservableCollection<string>(User32Test.GetOpenWindowsNames());
            if (settingService.ExcludedApps?.Applications != null)
            {
                ExcludedApplications = new ObservableCollection<string>(settingService.ExcludedApps.Applications);
            }
            else
            {
                ExcludedApplications = new ObservableCollection<string>();
            }

            ActivatedCommand = new DelegateCommand<Window>(async (mainWindow) =>
            {
                if (settingService.Settings.ShowMainWindow)
                {
                    settingService.Settings.ShowMainWindow = false;
                    HideWindowAtStartup = false;
                }
                else if (!DevMode.IsActive && HideWindowAtStartup)
                {
                    HideWindowAtStartup = false;
                    mainWindow.Hide();
                }

                IsStartupTaskActive = await settingService.GetStartupTaskStatusAsync();
            });

            CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);

            NewLayoutCommand = new DelegateCommand(() =>
            {
                var layout = new Layout
                {
                    Guid = Guid.NewGuid(),
                    IsSaved = false,
                    Name = "New layout"
                };

                Layouts.Add(layout);
                SelectedLayout = Layouts.FirstOrDefault(i => i.Guid == layout.Guid);

                var designWindow = new DesignWindow();
                designWindow.Closing += DesignWindow_Closing;
                designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
                designWindow.Show();
            });

            DesignLayoutCommand = new DelegateCommand(() =>
            {
                var designWindow = new DesignWindow();
                designWindow.Closing += DesignWindow_Closing;
                designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
                designWindow.Show();
            });

            ExportLayoutCommand = new DelegateCommand(() =>
            {
                var fileDialog = new SaveFileDialog
                {
                    DefaultExt = ".json",
                    Filter = "Json (.json)|*.json",
                    FileName = selectedLayout.Name
                };

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    settingService.ExportLayout(SelectedLayout, fileDialog.FileName);
                }
            });

            ImportLayoutCommand = new DelegateCommand(() =>
            {
                try
                {
                    var fileDialog = new OpenFileDialog
                    {
                        DefaultExt = ".json",
                        Filter = "Json (.json)|*.json"
                    };

                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var imported = settingService.ImportLayout(fileDialog.FileName);
                        Layouts.Insert(0, imported);
                    }
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Layout file seems to be corrupted. Please try again with other layout file.");
                }
            });

            OpenRenameDialogCommand = new DelegateCommand<Layout>((layout) =>
            {
                RenameLayout = layout;
                IsRenameDialogOpen = true;
            });

            ExcludeAppLayoutCommand = new DelegateCommand(() =>
            {
                ExcludedApplications.Add(SelectedApplication);
                SelectedApplication = null;

                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            },
            () =>
            {
                return !string.IsNullOrWhiteSpace(SelectedApplication);
            }).ObservesProperty(() => SelectedApplication);

            IncludeAppLayoutCommand = new DelegateCommand(() =>
            {
                ExcludedApplications.Remove(SelectedExcludedApplication);

                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            },
            () =>
            {
                return SelectedExcludedApplication != null;
            }).ObservesProperty(() => SelectedExcludedApplication);

            HandleLinkClick = new DelegateCommand<string>((url) =>
            {
                var ps = new ProcessStartInfo("http://" + url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            });

            MouseButtons = new ObservableCollection<MouseButton>
            {
                MouseButton.Left,
                MouseButton.Middle,
                MouseButton.Right
            };

            if (!DevMode.IsActive)
            {
                snapService.Initialize();
            }
        }

        private void DesignWindow_Closing(object sender, CancelEventArgs e)
        {
            settingService.SaveLayout(SelectedLayout);
            settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);

            var position = Layouts.IndexOf(SelectedLayout);

            Layouts.Remove(SelectedLayout);
            Layouts.Insert(position, SelectedLayout);

            ApplyChanges();
        }

        private void ApplyChanges()
        {
            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                settingService.Save();

                window.Hide();
            }

            if (DevMode.IsActive)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}