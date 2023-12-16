using System.Windows;
using SnapIt.Common.Mvvm;

namespace SnapIt.ViewModels.Dialogs;

public class RenameDialogViewModel : ViewModelBase
{
    private string layoutName;

    public string LayoutName { get => layoutName; set => SetProperty(ref layoutName, value); }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
    }
}