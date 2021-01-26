using System.Windows.Input;
using Prism.Regions;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.Views
{
    public partial class MainWindow
    {
        private IRegionManager regionManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Initialize(
            INotifyIconService notifyIconService,
            IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            notifyIconService.SetView += NotifyIconService_SetView;
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void NotifyIconService_SetView(ViewType viewType)
        {
            //LayoutViewRadio.IsChecked = false;
            //SettingsViewRadio.IsChecked = false;
            //AboutViewRadio.IsChecked = false;

            //switch (viewType)
            //{
            //    case ViewType.LayoutView:
            //        LayoutViewRadio.IsChecked = true;
            //        regionManager.RequestNavigate(Constants.MainRegion, "LayoutView");
            //        break;

            //    case ViewType.SettingsView:
            //        SettingsViewRadio.IsChecked = true;
            //        regionManager.RequestNavigate(Constants.MainRegion, "SettingsView");
            //        break;

            //    case ViewType.AboutView:
            //        AboutViewRadio.IsChecked = true;
            //        regionManager.RequestNavigate(Constants.MainRegion, "AboutView");
            //        break;
            //}
        }

        private void HamburgerMenuControl_ItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs args)
        {
        }
    }
}