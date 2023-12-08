using SnapIt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

public partial class KeyboardSettingsPage : INavigableView<KeyboardSettingsPageViewModel>
{
    public KeyboardSettingsPageViewModel ViewModel { get; }

    public KeyboardSettingsPage(KeyboardSettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}