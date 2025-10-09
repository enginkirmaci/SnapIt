using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Dialogs;

/// <summary>
/// Interaction logic for StoreWebsiteDialog.xaml
/// </summary>
public partial class StoreWebsiteDialog : ContentDialog
{
    public StoreWebsiteDialogViewModel? ViewModel { get; }

    public StoreWebsiteDialog(ContentPresenter contentPresenter)
    : base(contentPresenter)
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<StoreWebsiteDialogViewModel>();
        DataContext = this;
    }
}