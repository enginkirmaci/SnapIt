using Prism.Commands;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;
using SnapIt.Views.Pages;
using Wpf.Ui;

namespace SnapIt.ViewModels.Pages;

public class AboutPageViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;

    public DelegateCommand<string> HandleLinkClick { get; private set; }
    public DelegateCommand RateReviewStoreClick { get; private set; }
    public DelegateCommand WhatsNewClick { get; private set; }

    public AboutPageViewModel(
         INavigationService navigationService)
    {
        this.navigationService = navigationService;

        RateReviewStoreClick = new DelegateCommand(async () =>
        {
            try
            {
                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Rate and review only works for Windows 10 or later versions");
            }
        });

        WhatsNewClick = new DelegateCommand(() =>
        {
            navigationService.Navigate(typeof(WhatsNewPage));
        });
    }

    public override async Task InitializeAsync()
    {
    }
}