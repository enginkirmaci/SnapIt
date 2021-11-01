using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
    public class LayoutViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private ObservableCollectionWithItemNotify<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private ObservableCollection<Layout> layouts;
        private Layout selectedLayout;
        private bool isRenameDialogOpen;
        private Layout popupLayout;
        private string renameDialogLayoutName;

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

        public bool IsRenameDialogOpen { get => isRenameDialogOpen; set => SetProperty(ref isRenameDialogOpen, value); }
        public string RenameDialogLayoutName { get => renameDialogLayoutName; set => SetProperty(ref renameDialogLayoutName, value); }
        public Layout PopupLayout { get => popupLayout; set => SetProperty(ref popupLayout, value); }
        public SnapAreaTheme Theme { get; set; }
        public DelegateCommand NewLayoutCommand { get; private set; }
        public DelegateCommand ImportLayoutCommand { get; private set; }
        public DelegateCommand<Layout> DesignLayoutCommand { get; private set; }
        public DelegateCommand<Layout> OpenRenameDialogCommand { get; private set; }
        public DelegateCommand<object> RenameDialogClosingCommand { get; private set; }
        public DelegateCommand<Layout> DeleteLayoutCommand { get; private set; }
        public DelegateCommand<Layout> ExportLayoutCommand { get; private set; }

        public LayoutViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            snapService.ScreenChanged += SnapService_ScreenChanged;

            Theme = new SnapAreaTheme
            {
                HighlightColor = Color.FromArgb(200, 33, 33, 33),
                OverlayColor = Color.FromArgb(200, 99, 99, 99),
                BorderColor = Color.FromArgb(150, 200, 200, 200),
                BorderThickness = 1,
                Opacity = 1
            };

            foreach (var layout in settingService.Layouts)
            {
                layout.Theme = Theme;
            }

            Layouts = new ObservableCollection<Layout>(settingService.Layouts);
            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens.FirstOrDefault(); //TODO this causes two times initialization on startup

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
                    MessageBox.Show("Layout file seems to be corrupted. Please try again with other layout file.");
                }
            });

            OpenRenameDialogCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;
                RenameDialogLayoutName = PopupLayout.Name;
                IsRenameDialogOpen = true;
            });

            RenameDialogClosingCommand = new DelegateCommand<object>((isSave) =>
            {
                if ((bool)isSave)
                {
                    PopupLayout.Name = RenameDialogLayoutName;
                }

                IsRenameDialogOpen = false;
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

                //Layouts.Insert(0, layout);
                PopupLayout = layout; // Layouts.FirstOrDefault(i => i.Guid == layout.Guid);

                var designWindow = new DesignWindow();
                designWindow.Closed += DesignWindow_Closed;
                designWindow.SetViewModel(SelectedSnapScreen, PopupLayout);
                designWindow.Show();
            });

            DesignLayoutCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;

                var designWindow = new DesignWindow();
                designWindow.Closed += DesignWindow_Closed;
                designWindow.SetViewModel(SelectedSnapScreen, PopupLayout);
                designWindow.Show();
            });
        }

        private void SnapScreens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ApplyChanges();
        }

        private void SnapService_ScreenChanged(System.Collections.Generic.IList<SnapScreen> snapScreens)
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
            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }
    }
}