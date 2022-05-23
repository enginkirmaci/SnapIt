using System.Collections.ObjectModel;
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

        private ObservableCollectionWithItemNotify<ApplicationGroup> applicationGroups;
        private ApplicationGroup selectedApplicationGroup;
        private ApplicationItem selectedApplicationItem;
        private ApplicationItem unmodifiedApplicationItem;
        private bool isApplicationItemOpen;
        private bool isMoveApplicationItemOpen;
        private ObservableCollectionWithItemNotify<SnapScreen> snapScreens;
        private SnapScreen selectedSnapScreen;
        private SnapScreenViewer snapScreenViewer;
        private ObservableCollection<int> areaNumbers;

        public ObservableCollectionWithItemNotify<ApplicationGroup> ApplicationGroups { get => applicationGroups; set => SetProperty(ref applicationGroups, value); }
        public ObservableCollection<int> AreaNumbers { get => areaNumbers; set => SetProperty(ref areaNumbers, value); }
        public ApplicationGroup SelectedApplicationGroup { get => selectedApplicationGroup; set => SetProperty(ref selectedApplicationGroup, value); }

        public bool IsApplicationItemOpen { get => isApplicationItemOpen; set => SetProperty(ref isApplicationItemOpen, value); }
        public bool IsMoveApplicationItemOpen { get => isMoveApplicationItemOpen; set => SetProperty(ref isMoveApplicationItemOpen, value); }
        public ApplicationItem SelectedApplicationItem { get => selectedApplicationItem; set => SetProperty(ref selectedApplicationItem, value); }
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
        public DelegateCommand AddApplicationGroupCommand { get; private set; }
        public DelegateCommand BrowseApplicationItemCommand { get; private set; }
        public DelegateCommand ImportLayoutCommand { get; private set; }

        public ApplicationViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            snapService.ScreenChanged += SnapService_ScreenChanged;

            SnapScreens = new ObservableCollectionWithItemNotify<SnapScreen>(settingService.SnapScreens);
            SelectedSnapScreen = SnapScreens?.FirstOrDefault();

            LoadedCommand = new DelegateCommand<Page>((page) =>
            {
                snapScreenViewer = page.FindChild<SnapScreenViewer>("SnapScreenViewer");

                var applicationGroupList = page.FindChild<ListView>("ApplicationGroupList");
                applicationGroupList.SelectionChanged += ApplicationGroupList_SelectionChanged;
                ApplicationGroupList_SelectionChanged(null, null);
            });

            AreaHighlightCommand = new DelegateCommand<object>((number) =>
            {
                GetSelectSnapControl()?.HighlightArea((int)number);
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
                SelectedApplicationGroup = ApplicationGroups.FirstOrDefault();
            });

            BrowseApplicationItemCommand = new DelegateCommand(() =>
            {
                var openFileDialog = new System.Windows.Forms.OpenFileDialog();
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedApplicationItem.Path = openFileDialog.FileName;
                }
            });
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