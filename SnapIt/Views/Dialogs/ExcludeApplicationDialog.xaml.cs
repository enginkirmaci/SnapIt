using System.Windows.Controls;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Controls.Dialogs;

/// <summary>
/// Interaction logic for ExcludeApplicationDialog.xaml
/// </summary>
public partial class ExcludeApplicationDialog : ContentDialog
{
    public ExcludeApplicationDialogViewModel? ViewModel { get; }

    public ExcludeApplicationDialog(
        ContentPresenter contentPresenter)
        : base(contentPresenter)
    {
        ViewModel = App.AppContainer.GetService<ExcludeApplicationDialogViewModel>();
        DataContext = this;

        InitializeComponent();
    }

    protected override void OnButtonClick(ContentDialogButton button)
    {
        base.OnButtonClick(button);
        //if (CheckBox.IsChecked != false)
        //{
        //    base.OnButtonClick(button);
        //    return;
        //}
        //;

        //TextBlock.Visibility = Visibility.Visible;
        //CheckBox.Focus();
    }
}