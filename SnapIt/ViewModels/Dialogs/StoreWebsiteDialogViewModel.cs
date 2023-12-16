using System.Windows;
using SnapIt.Common.Mvvm;

namespace SnapIt.ViewModels.Dialogs;

public class StoreWebsiteDialogViewModel : ViewModelBase
{
    private string licenseMessageCloseButtonText;
    private bool isTrialEnded;

    public bool IsTrialEnded { get => isTrialEnded; set => SetProperty(ref isTrialEnded, value); }
    public string LicenseMessageCloseButtonText { get => licenseMessageCloseButtonText; set => SetProperty(ref licenseMessageCloseButtonText, value); }

    public StoreWebsiteDialogViewModel()
    {
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        LicenseMessageCloseButtonText = IsTrialEnded ? "Exit Application" : "Close";
    }
}