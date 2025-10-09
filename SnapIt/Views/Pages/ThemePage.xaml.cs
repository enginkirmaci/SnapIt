using System.Windows.Controls;
using SnapIt.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace SnapIt.Views.Pages;

/// <summary>
/// Interaction logic for ThemeView.xaml
/// </summary>
public partial class ThemePage : INavigableView<ThemePageViewModel>
{
    public ThemePageViewModel ViewModel { get; }

    public ThemePage(ThemePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}