using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SnapIt.ViewModels.Dialogs;
using Wpf.Ui.Controls;

namespace SnapIt.Views.Dialogs;

/// <summary>
/// Interaction logic for NewVersionDialog.xaml
/// </summary>
public partial class NewVersionDialog : ContentDialog
{
    public RenameDialogViewModel? ViewModel { get; }

    public NewVersionDialog(
    ContentPresenter contentPresenter)
    : base(contentPresenter)
    {
        ViewModel = App.AppContainer.GetService<RenameDialogViewModel>();
        DataContext = this;

        InitializeComponent();
    }
}