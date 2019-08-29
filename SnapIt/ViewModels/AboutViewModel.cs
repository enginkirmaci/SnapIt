using System;
using Prism.Commands;
using Prism.Mvvm;

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
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9PHGBMZ7RBZX"));
            });

            HandleLinkClick = new DelegateCommand<string>(async (url) =>
            {
                string uriToLaunch = $"http://{url}";
                var uri = new Uri(uriToLaunch);

                await Windows.System.Launcher.LaunchUriAsync(uri);
            });
        }
    }
}