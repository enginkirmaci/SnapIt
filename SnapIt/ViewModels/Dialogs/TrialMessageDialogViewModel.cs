using System.Windows;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;

namespace SnapIt.ViewModels.Dialogs;

public class TrialMessageDialogViewModel : ViewModelBase
{
    private string licenseMessageCloseButtonText;
    private string titleText;
    private bool isTrialEnded;

    public string LicenseMessageCloseButtonText { get => licenseMessageCloseButtonText; set => SetProperty(ref licenseMessageCloseButtonText, value); }
    public string TitleText { get => titleText; set => SetProperty(ref titleText, value); }
    public bool IsTrialEnded { get => isTrialEnded; set => SetProperty(ref isTrialEnded, value); }

    public TrialMessageDialogViewModel()
    {
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        TitleText = $"{Constants.AppName} Trial";
        LicenseMessageCloseButtonText = IsTrialEnded ? "Exit Application" : "Close";
    }
}