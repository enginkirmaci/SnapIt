using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Dialogs;

/// <summary>
/// Interaction logic for RenameDialog.xaml
/// </summary>
public partial class RenameDialog : ContentDialog
{
    public RenameDialogViewModel? ViewModel { get; }

    public RenameDialog(
        ContentPresenter contentPresenter)
        : base(contentPresenter)
    {
        ViewModel = App.Services.GetRequiredService<RenameDialogViewModel>();
        DataContext = this;

        InitializeComponent();
    }
}