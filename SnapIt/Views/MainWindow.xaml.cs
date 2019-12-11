using System.Windows;
using Prism.Regions;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IRegionManager regionManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetNotifyIconService(
            INotifyIconService notifyIconService,
            IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            notifyIconService.SetView += NotifyIconService_SetView;
        }

        private void NotifyIconService_SetView(ViewType viewType)
        {
            LayoutViewRadio.IsChecked = false;
            SettingsViewRadio.IsChecked = false;
            AboutViewRadio.IsChecked = false;

            switch (viewType)
            {
                case ViewType.LayoutView:
                    LayoutViewRadio.IsChecked = true;
                    regionManager.RequestNavigate(Constants.MainRegion, "LayoutView");
                    break;

                case ViewType.SettingsView:
                    SettingsViewRadio.IsChecked = true;
                    regionManager.RequestNavigate(Constants.MainRegion, "SettingsView");
                    break;

                case ViewType.AboutView:
                    AboutViewRadio.IsChecked = true;
                    regionManager.RequestNavigate(Constants.MainRegion, "AboutView");
                    break;
            }
        }
    }
}