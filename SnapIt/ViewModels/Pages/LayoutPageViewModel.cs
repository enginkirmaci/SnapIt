using System.Collections.Specialized;
using System.Windows;
using System.Windows.Forms;
using Prism.Commands;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;
using SnapIt.ViewModels.Windows;
using SnapIt.Views.Dialogs;
using SnapIt.Views.Windows;
using Wpf.Ui;

namespace SnapIt.ViewModels.Pages
{
    public class LayoutPageViewModel : ViewModelBase
    {
        private readonly ISnapManager snapManager;
        private readonly ISettingService settingService;
        private ObservableCollectionWithItemNotify<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private ObservableCollection<Layout> layouts;
        private Layout selectedLayout;
        private Layout popupLayout;

        public ObservableCollectionWithItemNotify<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }

        public SnapScreen SelectedSnapScreen
        {
            get => selectedSnapScreen;
            set
            {
                SetProperty(ref selectedSnapScreen, value);
                SelectedLayout = selectedSnapScreen?.Layout;
            }
        }

        public ObservableCollection<Layout> Layouts { get => layouts; set => SetProperty(ref layouts, value); }

        public Layout SelectedLayout
        {
            get => selectedLayout;
            set
            {
                SetProperty(ref selectedLayout, value);

                if (value != null)
                {
                    SelectedSnapScreen.Layout = selectedLayout;
                    settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);
                    ApplyChanges();
                }
            }
        }

        public Layout PopupLayout { get => popupLayout; set => SetProperty(ref popupLayout, value); }

        public SnapAreaTheme Theme { get; set; }
        public DelegateCommand NewLayoutCommand { get; private set; }
        public DelegateCommand ImportLayoutCommand { get; private set; }
        public DelegateCommand<Layout> DesignLayoutCommand { get; private set; }
        public DelegateCommand<Layout> OpenRenameDialogCommand { get; private set; }
        public DelegateCommand<Layout> DeleteLayoutCommand { get; private set; }
        public DelegateCommand<Layout> ExportLayoutCommand { get; private set; }

        public LayoutPageViewModel(
            ISnapManager snapManager,
            ISettingService settingService,
            IContentDialogService contentDialogService,
            DesignWindowViewModel designWindowViewModel)
        {
            this.snapManager = snapManager;
            this.settingService = settingService;

            snapManager.ScreenChanged += SnapService_ScreenChanged;

            Layouts = new ObservableCollection<Layout>(settingService.Layouts);
            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens.FirstOrDefault();

            SnapScreens.CollectionChanged += SnapScreens_CollectionChanged;

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
                    System.Windows.MessageBox.Show("Layout file seems to be corrupted. Please try again with other layout file.");
                }
            });

            OpenRenameDialogCommand = new DelegateCommand<Layout>(async (layout) =>
            {
                var renameDialog = new RenameDialog(contentDialogService.GetContentPresenter())
                {
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonText = "Save"
                };

                renameDialog.ViewModel.LayoutName = layout.Name;

                var result = await renameDialog.ShowAsync();

                if (result == Wpf.Ui.Controls.ContentDialogResult.Primary)
                {
                    layout.Name = renameDialog.ViewModel.LayoutName;
                }
            });

            DeleteLayoutCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;

                Layouts.Remove(PopupLayout);

                settingService.DeleteLayout(PopupLayout);

                if (SelectedLayout != null && !Layouts.Contains(SelectedLayout))
                {
                    SelectedLayout = Layouts.FirstOrDefault();
                }
            });

            ExportLayoutCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;

                var fileDialog = new SaveFileDialog
                {
                    DefaultExt = ".json",
                    Filter = "Json (.json)|*.json",
                    FileName = PopupLayout.Name
                };

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    settingService.ExportLayout(PopupLayout, fileDialog.FileName);
                }
            });

            NewLayoutCommand = new DelegateCommand(() =>
            {
                var layout = new Layout
                {
                    Guid = Guid.NewGuid(),
                    IsNew = true,
                    Status = LayoutStatus.NotSaved,
                    Name = "New layout",
                    Theme = Theme
                };

                PopupLayout = layout;

                var designWindow = new DesignWindow(designWindowViewModel);
                designWindow.Closed += DesignWindow_Closed;
                designWindow.SetViewModel(SelectedSnapScreen, PopupLayout);
                designWindow.Show();
            });

            DesignLayoutCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;

                var designWindow = new DesignWindow(designWindowViewModel);
                designWindow.Closed += DesignWindow_Closed;
                designWindow.SetViewModel(SelectedSnapScreen, PopupLayout);
                designWindow.Show();
            });
        }

        public override async Task InitializeAsync(RoutedEventArgs args)
        {
            await settingService.InitializeAsync();
        }

        private void SnapScreens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ApplyChanges();
        }

        private void SnapService_ScreenChanged(IList<SnapScreen> snapScreens)
        {
            try
            {
                if (selectedSnapScreen != null)
                {
                    var deviceNumber = selectedSnapScreen.DeviceNumber;

                    SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
                    SelectedSnapScreen = SnapScreens.FirstOrDefault(s => s.DeviceNumber == deviceNumber);
                }
            }
            catch { }
        }

        private void DesignWindow_Closed(object sender, EventArgs e)
        {
            if (PopupLayout.Status == LayoutStatus.NotSaved)
            {
                if (PopupLayout.IsNew)
                {
                    PopupLayout.IsNew = false;

                    settingService.Layouts.Insert(0, PopupLayout);
                    Layouts = new ObservableCollection<Layout>(settingService.Layouts);
                }
                else
                {
                    var position = Layouts.IndexOf(PopupLayout);

                    Layouts.Remove(PopupLayout);
                    Layouts.Insert(position, PopupLayout);

                    var selected = SnapScreens.FirstOrDefault(item => item.Layout == PopupLayout);
                    if (selected != null)
                    {
                        selected.Layout = null;
                        selected.Layout = PopupLayout;
                    }
                }

                if (SelectedLayout == null)
                {
                    SelectedLayout = PopupLayout;
                }

                ApplyChanges();
            }
        }

        private void ApplyChanges()
        {
            if (!Dev.IsActive)
            {
                snapManager.Dispose();
                _ = snapManager.InitializeAsync();
            }
        }
    }
}