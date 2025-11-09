using SnapIt.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace SnapIt.Views.Pages;

/// <summary>
/// Interaction logic for AboutView.xaml
/// </summary>
public partial class AboutPage : INavigableView<AboutPageViewModel>
{
    public AboutPageViewModel ViewModel { get; }

    public AboutPage(AboutPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}