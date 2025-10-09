using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Dialogs;

/// <summary>
/// Interaction logic for NewVersionDialog.xaml
/// </summary>
public partial class NewVersionDialog : ContentDialog
{
    public RenameDialogViewModel? ViewModel { get; }

    public NewVersionDialog(
    ContentPresenter contentPresenter)
    : base(contentPresenter)
    {
        ViewModel = App.Services.GetRequiredService<RenameDialogViewModel>();
        DataContext = this;

        InitializeComponent();
    }
}