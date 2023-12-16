using SnapIt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

/// <summary>
/// Interaction logic for WhatsNewView.xaml
/// </summary>
public partial class WhatsNewPage : INavigableView<WhatsNewPageViewModel>
{
    public WhatsNewPageViewModel ViewModel { get; }

    public WhatsNewPage(WhatsNewPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}