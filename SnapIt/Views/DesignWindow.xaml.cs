using System.Windows;
using SnapIt.Library;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.ViewModels;

namespace SnapIt.Views
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : Window
    {
        public DesignWindow()
        {
            InitializeComponent();
        }

        public void SetViewModel(SnapScreen snapScreen, Layout layout)
        {
            var model = DataContext as DesignWindowViewModel;
            model.Window = this;
            model.SnapScreen = snapScreen;
            model.Layout = layout;

            if (DevMode.IsActive)
            {
                Topmost = false;
            }
        }
    }
}