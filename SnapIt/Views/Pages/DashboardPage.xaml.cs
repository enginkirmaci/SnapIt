using SnapIt.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace SnapIt.Views.Pages;

public partial class DashboardPage : INavigableView<DashboardPageViewModel>
{
    public DashboardPageViewModel ViewModel { get; }

    public DashboardPage(DashboardPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}