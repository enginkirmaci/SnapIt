using SnapIt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

public partial class WindowsPage : INavigableView<WindowsPageViewModel>
{
    public WindowsPageViewModel ViewModel { get; }

    public WindowsPage(WindowsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}