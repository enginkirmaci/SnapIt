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
            var index = 0;
            foreach (MahApps.Metro.Controls.HamburgerMenuGlyphItem item in (MahApps.Metro.Controls.HamburgerMenuItemCollection)HamburgerMenuControl.ItemsSource)
            {
                if (item.CommandParameter.Equals(viewType.ToString()))
                {
                    HamburgerMenuControl.SelectedIndex = index;
                    regionManager.RequestNavigate(Constants.MainRegion, viewType.ToString());

                    break;
                }

                index++;
            }
        }
    }
}