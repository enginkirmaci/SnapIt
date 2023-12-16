using System.Windows;
using Prism.Mvvm;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;

namespace SnapIt.ViewModels.Dialogs;

public class RunningApplicationsDialogViewModel : ViewModelBase
{
    private readonly IWinApiService winApiService;
    private string selectedApplication;
    private ObservableCollection<string> runningApplications;

    public string SelectedApplication { get => selectedApplication; set => SetProperty(ref selectedApplication, value); }
    public ObservableCollection<string> RunningApplications { get => runningApplications; set => SetProperty(ref runningApplications, value); }

    public RunningApplicationsDialogViewModel(IWinApiService winApiService)
    {
        this.winApiService = winApiService;
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        RunningApplications = new ObservableCollection<string>(winApiService.GetOpenWindowsNames());
    }
}