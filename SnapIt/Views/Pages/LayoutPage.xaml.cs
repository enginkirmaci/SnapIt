using SnapIt.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

/// <summary>
/// Interaction logic for LayoutView.xaml
/// </summary>
public partial class LayoutPage : INavigableView<LayoutPageViewModel>
{
    public LayoutPageViewModel ViewModel { get; }

    public LayoutPage(LayoutPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }

    private void editButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var menu = (sender as Button).ContextMenu;
        //menu.DataContext = DataContext;
        menu.IsOpen = true;
    }
}