using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SnapIt.Library.Entities;
using System;
using System.Diagnostics;

namespace SnapIt.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand RateReviewStoreClick { get; private set; }
        public DelegateCommand WhatsNewClick { get; private set; }

        public AboutViewModel(
             IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            RateReviewStoreClick = new DelegateCommand(async () =>
            {
                try
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("Rate and review only works for Windows 10 or later versions");
                }
            });

            HandleLinkClick = new DelegateCommand<string>((url) =>
            {
                string uriToLaunch = $"http://{url}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = uriToLaunch,
                    UseShellExecute = true
                });
            });

            WhatsNewClick = new DelegateCommand(() =>
            {
                this.regionManager.RequestNavigate(Constants.MainRegion, "WhatsNewView");
            });
        }
    }
}