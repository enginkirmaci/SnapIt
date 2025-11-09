using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Dialogs;

/// <summary>
/// Interaction logic for TrialMessageDialog.xaml
/// </summary>
public partial class TrialMessageDialog : ContentDialog
{
    public TrialMessageDialogViewModel? ViewModel { get; }

    public TrialMessageDialog(
        ContentPresenter contentPresenter)
        : base(contentPresenter)
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<TrialMessageDialogViewModel>();
        DataContext = this;
    }
}