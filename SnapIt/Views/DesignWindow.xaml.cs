using System.Windows;
using SnapIt.Library;
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

            MouseMove += DesignWindow_MouseMove;
        }

        private void DesignWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Point myPoint = e.GetPosition(this);
            mousePositionText.Text = $"Mouse Position: {myPoint.X:0.00} x {myPoint.Y:0.00}";
            horizontalRuler.RaiseHorizontalRulerMoveEvent(e);
            verticalRuler.RaiseVerticalRulerMoveEvent(e);
        }

        public void SetViewModel(SnapScreen snapScreen, Layout layout)
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