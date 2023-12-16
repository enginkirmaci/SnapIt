using SnapIt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

public partial class MouseSettingsPage : INavigableView<MouseSettingsPageViewModel>
{
    public MouseSettingsPageViewModel ViewModel { get; }

    public MouseSettingsPage(MouseSettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}