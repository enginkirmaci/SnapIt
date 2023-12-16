using System.Windows.Controls;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Dialogs;

/// <summary>
/// Interaction logic for RunningApplicationsDialog.xaml
/// </summary>
public partial class RunningApplicationsDialog : ContentDialog
{
    public RunningApplicationsDialogViewModel? ViewModel { get; }

    public RunningApplicationsDialog(ContentPresenter contentPresenter)
        : base(contentPresenter)
    {
        ViewModel = App.AppContainer.GetService<RunningApplicationsDialogViewModel>();
        DataContext = this;

        InitializeComponent();
    }
}