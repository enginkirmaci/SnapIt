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
/// Interaction logic for StoreWebsiteDialog.xaml
/// </summary>
public partial class StoreWebsiteDialog : ContentDialog
{
    public StoreWebsiteDialogViewModel? ViewModel { get; }

    public StoreWebsiteDialog(ContentPresenter contentPresenter)
    : base(contentPresenter)
    {
        InitializeComponent();

        ViewModel = App.AppContainer.GetService<StoreWebsiteDialogViewModel>();
        DataContext = this;
    }
}