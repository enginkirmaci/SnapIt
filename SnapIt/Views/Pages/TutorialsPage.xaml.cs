using SnapIt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Pages;

public partial class TutorialsPage : INavigableView<TutorialsPageViewModel>
{
    public TutorialsPageViewModel ViewModel { get; }

    public TutorialsPage(TutorialsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();
    }
}