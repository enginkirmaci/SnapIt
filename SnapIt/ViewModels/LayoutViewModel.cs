using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
    public class LayoutViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;
        private readonly IWinApiService winApiService;

        private ObservableCollection<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private ObservableCollection<Layout> layouts;
        private Layout selectedLayout;
        private bool isRenameDialogOpen;
        private Layout popupLayout;

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

        public bool IsRenameDialogOpen { get => isRenameDialogOpen; set => SetProperty(ref isRenameDialogOpen, value); }
        public Layout PopupLayout { get => popupLayout; set => SetProperty(ref popupLayout, value); }

        public DelegateCommand NewLayoutCommand { get; private set; }
        public DelegateCommand ImportLayoutCommand { get; private set; }
        public DelegateCommand<Layout> DesignLayoutCommand { get; private set; }
        public DelegateCommand<Layout> OpenRenameDialogCommand { get; private set; }
        public DelegateCommand<Layout> DeleteLayoutCommand { get; private set; }
        public DelegateCommand<Layout> ExportLayoutCommand { get; private set; }

        public LayoutViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IWinApiService winApiService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
            this.winApiService = winApiService;

            Layouts = new ObservableCollection<Layout>(settingService.Layouts);
            SnapScreens = new ObservableCollection<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens.FirstOrDefault();

            NewLayoutCommand = new DelegateCommand(() =>
            {
                var layout = new Layout
                {
                    Guid = Guid.NewGuid(),
                    IsSaved = false,
                    Name = "New layout"
                };

                Layouts.Insert(0, layout);
                PopupLayout = Layouts.FirstOrDefault(i => i.Guid == layout.Guid);

                var designWindow = new DesignWindow(winApiService);
                designWindow.Closing += DesignWindow_Closing;
                designWindow.SetScreen(SelectedSnapScreen, PopupLayout);
                designWindow.Show();
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
                    MessageBox.Show("Layout file seems to be corrupted. Please try again with other layout file.");
                }
            });

            DesignLayoutCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;

                var designWindow = new DesignWindow(winApiService);
                designWindow.Closing += DesignWindow_Closing;
                designWindow.SetScreen(SelectedSnapScreen, PopupLayout);
                designWindow.Show();
            });

            OpenRenameDialogCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;
                IsRenameDialogOpen = true;
            });

            DeleteLayoutCommand = new DelegateCommand<Layout>((layout) =>
            {
                PopupLayout = layout;

                Layouts.Remove(PopupLayout);

                if (SelectedLayout == null)
                {
                    SelectedLayout = Layouts.FirstOrDefault();
                }

                settingService.DeleteLayout(PopupLayout);
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
        }

        private void DesignWindow_Closing(object sender, CancelEventArgs e)
        {
            settingService.SaveLayout(PopupLayout);

            var position = Layouts.IndexOf(PopupLayout);

            Layouts.Remove(PopupLayout);
            Layouts.Insert(position, PopupLayout);

            SelectedLayout = PopupLayout;

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
    }
}