using Prism.Mvvm;
using SnapIt.Services.Contracts;

namespace SnapIt.ViewModels.Dialogs;

public class RunningApplicationsDialogViewModel : BindableBase
{
    private readonly IWinApiService winApiService;
    private string selectedApplication;
    private ObservableCollection<string> runningApplications;

    public string SelectedApplication { get => selectedApplication; set => SetProperty(ref selectedApplication, value); }
    public ObservableCollection<string> RunningApplications { get => runningApplications; set => SetProperty(ref runningApplications, value); }

    public RunningApplicationsDialogViewModel(IWinApiService winApiService)
    {
        this.winApiService = winApiService;

        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        RunningApplications = new ObservableCollection<string>(winApiService.GetOpenWindowsNames());
    }
}