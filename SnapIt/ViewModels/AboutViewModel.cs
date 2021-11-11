using System;
using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand RateReviewStoreClick { get; private set; }

        public AboutViewModel()
        {
            RateReviewStoreClick = new DelegateCommand(async () =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
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
        }
    }
}