using System.Windows;
using SnapScreen.Library;
using SnapScreen.Library.Entities;
using SnapScreen.ViewModels;

namespace SnapScreen.Views
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

        public void SetViewModel(Library.Entities.SnapScreen snapScreen, Layout layout)
        {
            var model = DataContext as DesignWindowViewModel;
            if (model != null)
            {
                model.Window = this;
                model.SnapScreen = snapScreen;
                model.Layout = layout;
            }

            if (DevMode.IsTopmostDisabled)
            {
                Topmost = false;
            }
        }
    }
}