using Prism.Mvvm;

namespace SnapIt.ViewModels.Dialogs;

public class RenameDialogViewModel : BindableBase
{
    private string layoutName;

    public string LayoutName { get => layoutName; set => SetProperty(ref layoutName, value); }

    public RenameDialogViewModel()
    {
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
    }
}