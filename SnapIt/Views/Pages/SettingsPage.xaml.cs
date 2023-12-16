using SnapIt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

/// <summary>
/// Interaction logic for SettingsView.xaml
/// </summary>
public partial class SettingsPage : INavigableView<SettingsPageViewModel>
{
    public SettingsPageViewModel ViewModel { get; }

    public SettingsPage(SettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}