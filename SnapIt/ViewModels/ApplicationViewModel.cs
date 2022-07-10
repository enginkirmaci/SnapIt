using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class ApplicationViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;
        private readonly IApplicationService applicationService;
        private ObservableCollectionWithItemNotify<ApplicationGroup> applicationGroups;
        private ObservableCollection<ApplicationItem> listApplicationItem;
        private ObservableCollection<ApplicationItem> filteredlistApplicationItem;
        private ApplicationGroup selectedApplicationGroup;
        private ApplicationItem selectedApplicationItem;
        private ApplicationItem selectedListApplicationItem;
        private ApplicationItem unmodifiedApplicationItem;
        private bool isApplicationItemOpen;
        private bool isMoveApplicationItemOpen;
        private bool isListApplicationItemDialogOpen;
        private ObservableCollectionWithItemNotify<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private SnapScreenViewer snapScreenViewer;
        public ScrollViewer pageScrollViewer;
        private ObservableCollection<int> areaNumbers;
        private string titleFilter;

        public ObservableCollectionWithItemNotify<ApplicationGroup> ApplicationGroups { get => applicationGroups; set => SetProperty(ref applicationGroups, value); }
        public ObservableCollection<ApplicationItem> ListApplicationItem { get => listApplicationItem; set => SetProperty(ref listApplicationItem, value); }
        public ObservableCollection<ApplicationItem> FilteredlistApplicationItem { get => filteredlistApplicationItem; set => SetProperty(ref filteredlistApplicationItem, value); }
        public ObservableCollection<int> AreaNumbers { get => areaNumbers; set => SetProperty(ref areaNumbers, value); }

        public ApplicationGroup SelectedApplicationGroup
        {
            get => selectedApplicationGroup;
            set => SetProperty(ref selectedApplicationGroup, value);
        }

        public string TitleFilter
        {
            get => titleFilter;
            set
            {
                SetProperty(ref titleFilter, value);

                DevMode.Log($"{titleFilter}, {TitleFilter}");
                if (string.IsNullOrWhiteSpace(titleFilter))
                {
                    FilteredlistApplicationItem = ListApplicationItem;
                }
                else
                {
                    FilteredlistApplicationItem = new ObservableCollection<ApplicationItem>(ListApplicationItem.Where(i => i.Title.Contains(titleFilter, System.StringComparison.OrdinalIgnoreCase)));
                }
            }
        }

        public bool IsApplicationItemOpen { get => isApplicationItemOpen; set => SetProperty(ref isApplicationItemOpen, value); }
        public bool IsMoveApplicationItemOpen { get => isMoveApplicationItemOpen; set => SetProperty(ref isMoveApplicationItemOpen, value); }
        public bool IsListApplicationItemDialogOpen { get => isListApplicationItemDialogOpen; set => SetProperty(ref isListApplicationItemDialogOpen, value); }
        public ApplicationItem SelectedApplicationItem { get => selectedApplicationItem; set => SetProperty(ref selectedApplicationItem, value); }
        public ApplicationItem SelectedListApplicationItem { get => selectedListApplicationItem; set => SetProperty(ref selectedListApplicationItem, value); }

        public ObservableCollectionWithItemNotify<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }

        public SnapScreen SelectedSnapScreen
        {
            get => selectedSnapScreen; set
            {
                SetProperty(ref selectedSnapScreen, value);

                ApplicationGroups = new ObservableCollectionWithItemNotify<ApplicationGroup>(selectedSnapScreen.ApplicationGroups);

                SelectedApplicationGroup = ApplicationGroups?.FirstOrDefault();
            }
        }

        public SnapAreaTheme Theme { get; set; } = new SnapAreaTheme();
        public DelegateCommand<Page> LoadedCommand { get; private set; }
        public DelegateCommand<object> AreaHighlightCommand { get; private set; }
        public DelegateCommand<object> EditAreaItemCommand { get; private set; }
        public DelegateCommand<object> MoveAreaItemCommand { get; private set; }
        public DelegateCommand<object> DeleteAreaItemCommand { get; private set; }
        public DelegateCommand<object> EditApplicationItemDialogCommand { get; private set; }
        public DelegateCommand<object> MoveAreaItemDialogCommand { get; private set; }
        public DelegateCommand<object> AddApplicationItemDialogCommand { get; private set; }
        public DelegateCommand<object> CloseListApplicationItemDialogCommand { get; private set; }
        public DelegateCommand AddApplicationGroupCommand { get; private set; }
        public DelegateCommand DeleteApplicationGroupCommand { get; private set; }
        public DelegateCommand BrowseApplicationItemCommand { get; private set; }
        public DelegateCommand ListApplicationItemCommand { get; private set; }
        public DelegateCommand ImportLayoutCommand { get; private set; }

        public ApplicationViewModel(
            ISnapService snapService,
            ISettingService settingService,
            IApplicationService applicationService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
            this.applicationService = applicationService;
            snapService.ScreenChanged += SnapService_ScreenChanged;

            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens?.FirstOrDefault();

            LoadedCommand = new DelegateCommand<Page>((page) =>
            {
                snapScreenViewer = page.FindChild<SnapScreenViewer>("SnapScreenViewer");
                pageScrollViewer = page.FindChild<ScrollViewer>("PageScrollViewer");

                var applicationGroupList = page.FindChild<ListView>("ApplicationGroupList");
                applicationGroupList.SelectionChanged += ApplicationGroupList_SelectionChanged;
                ApplicationGroupList_SelectionChanged(null, null);

                SelectedApplicationGroup.PropertyChanged += SelectedApplicationGroup_PropertyChanged;
            });

            AreaHighlightCommand = new DelegateCommand<object>((number) =>
            {
                GetSelectSnapControl()?.HighlightArea((int)number);
                pageScrollViewer?.ScrollToTop();
            });

            EditAreaItemCommand = new DelegateCommand<object>((item) =>
            {
                IsApplicationItemOpen = true;
                SelectedApplicationItem = (ApplicationItem)item;

                unmodifiedApplicationItem = new ApplicationItem(SelectedApplicationItem);
            });

            MoveAreaItemCommand = new DelegateCommand<object>((item) =>
            {
                IsMoveApplicationItemOpen = true;
                SelectedApplicationItem = (ApplicationItem)item;

                unmodifiedApplicationItem = new ApplicationItem(SelectedApplicationItem);
            });

            DeleteAreaItemCommand = new DelegateCommand<object>((item) =>
            {
                var applicationItem = (ApplicationItem)item;
                var area = SelectedApplicationGroup.ApplicationAreas.FirstOrDefault(i => i.Number == applicationItem.AreaNumber);

                if (area != null)
                {
                    area.Applications.Remove(applicationItem);

                    ApplyChanges();
                }
            });

            EditApplicationItemDialogCommand = new DelegateCommand<object>((isSave) =>
            {
                IsApplicationItemOpen = false;

                var area = SelectedApplicationGroup.ApplicationAreas.FirstOrDefault(i => i.Number == SelectedApplicationItem.AreaNumber);

                if (area != null)
                {
                    var item = area.Applications.FirstOrDefault(i => i.Guid == SelectedApplicationItem.Guid);

                    if (item != null)
                    {
                        if ((bool)isSave)
                        {
                            if (string.IsNullOrEmpty(item.Title))
                            {
                                try
                                {
                                    var fileVersion = FileVersionInfo.GetVersionInfo(item.Path);
                                    item.Title = !string.IsNullOrWhiteSpace(fileVersion.ProductName) ? fileVersion.ProductName : fileVersion.FileDescription;
                                }
                                catch { }
                            }

                            var appPos = area.Applications.IndexOf(item);
                            area.Applications.RemoveAt(appPos);
                            area.Applications.Insert(appPos, item);

                            if (SelectedSnapScreen != null)
                            {
                                SelectedSnapScreen.ApplicationGroups = ApplicationGroups.ToList();
                                settingService.LinkScreenApplicationGroups(SelectedSnapScreen, ApplicationGroups.ToList());

                                ApplyChanges();
                            }
                        }
                        else
                        {
                            area.Applications.Remove(item);

                            if (unmodifiedApplicationItem != null)
                            {
                                area.Applications.Add(unmodifiedApplicationItem);
                            }
                        }
                    }
                }
            });

            MoveAreaItemDialogCommand = new DelegateCommand<object>((isMove) =>
            {
                IsMoveApplicationItemOpen = false;

                if ((bool)isMove)
                {
                    var area = SelectedApplicationGroup.ApplicationAreas.FirstOrDefault(i => i.Number == unmodifiedApplicationItem.AreaNumber);

                    if (area != null)
                    {
                        var item = area.Applications.FirstOrDefault(i => i.Guid == unmodifiedApplicationItem.Guid);

                        if (item != null)
                        {
                            area.Applications.Remove(item);

                            var newArea = SelectedApplicationGroup.ApplicationAreas.FirstOrDefault(i => i.Number == SelectedApplicationItem.AreaNumber);
                            if (newArea != null)
                            {
                                newArea.Applications.Add(SelectedApplicationItem);

                                ApplyChanges();
                            }
                        }
                    }
                }
            });

            AddApplicationItemDialogCommand = new DelegateCommand<object>((area) =>
            {
                unmodifiedApplicationItem = null;
                var applicationArea = (ApplicationArea)area;

                if (applicationArea.Applications == null)
                {
                    applicationArea.Applications = new ObservableCollectionWithItemNotify<ApplicationItem>();
                }

                var item = new ApplicationItem
                {
                    Guid = System.Guid.NewGuid(),
                    AreaNumber = applicationArea.Number
                };
                applicationArea.Applications.Add(item);

                IsApplicationItemOpen = true;
                SelectedApplicationItem = item;
            });

            AddApplicationGroupCommand = new DelegateCommand(() =>
            {
                ApplicationGroups.Add(new ApplicationGroup());
                SelectedApplicationGroup = ApplicationGroups?.FirstOrDefault();
            });

            DeleteApplicationGroupCommand = new DelegateCommand(() =>
            {
                ApplicationGroups.Remove(SelectedApplicationGroup);
                ApplyChanges();
            });

            BrowseApplicationItemCommand = new DelegateCommand(() =>
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog();
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedApplicationItem.Path = openFileDialog.FileName;
                }
            });

            ListApplicationItemCommand = new DelegateCommand(() =>
            {
                IsListApplicationItemDialogOpen = true;

                ListApplicationItem = new ObservableCollection<ApplicationItem>(applicationService.ListInstalledApplications());
                TitleFilter = string.Empty;
            });

            CloseListApplicationItemDialogCommand = new DelegateCommand<object>((isSelected) =>
            {
                IsListApplicationItemDialogOpen = false;

                if ((bool)isSelected)
                {
                    SelectedApplicationItem.Title = SelectedListApplicationItem.Title;
                    SelectedApplicationItem.Arguments = SelectedListApplicationItem.Arguments;
                    SelectedApplicationItem.Path = SelectedListApplicationItem.Path;
                }
            });
        }

        private void SelectedApplicationGroup_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActivateHotkey" || e.PropertyName == "Name")
            {
                ApplyChanges();
            }
        }

        private void ApplicationGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var snapControl = GetSelectSnapControl();
            if (snapControl != null)
            {
                var areaCount = snapControl.AreaCount;

                if (SelectedApplicationGroup != null)
                {
                    if (SelectedApplicationGroup.ApplicationAreas.Count <= areaCount)
                    {
                        for (int i = SelectedApplicationGroup.ApplicationAreas.Count; i < areaCount; i++)
                        {
                            SelectedApplicationGroup.ApplicationAreas.Add(new ApplicationArea(i + 1));
                        }
                    }
                    else
                    {
                        while (SelectedApplicationGroup.ApplicationAreas.Count - areaCount > 0)
                        {
                            SelectedApplicationGroup.ApplicationAreas.RemoveAt(SelectedApplicationGroup.ApplicationAreas.Count - 1);
                        }
                    }

                    AreaNumbers = new ObservableCollection<int>();
                    for (var i = 0; i < areaCount; i++)
                    {
                        AreaNumbers.Add(i + 1);
                    }
                }
            }
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

        private SnapControl GetSelectSnapControl()
        {
            var snapControls = snapScreenViewer.FindChildren<SnapControl>();

            var selectedSnapControl = snapControls?.FirstOrDefault(i => i.Layout == SelectedSnapScreen?.Layout);

            return selectedSnapControl;
        }

        private void ApplyChanges()
        {
            if (SelectedSnapScreen != null)
            {
                SelectedSnapScreen.ApplicationGroups = ApplicationGroups.ToList();
                settingService.LinkScreenApplicationGroups(SelectedSnapScreen, ApplicationGroups.ToList());
            }

            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }
    }
}