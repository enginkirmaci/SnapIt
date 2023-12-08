using System.Windows;
using SnapIt.Common;
using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.ViewModels.Windows;

namespace SnapIt.Views.Windows;

public partial class DesignWindow : IWindow
{
    public DesignWindowViewModel ViewModel { get; }

    public DesignWindow(
        DesignWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        MouseMove += DesignWindow_MouseMove;
    }

    private void DesignWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Point myPoint = e.GetPosition(this);
        mousePositionText.Text = $"Mouse Position  {myPoint.X:0.00} x {myPoint.Y:0.00}";

        horizontalRuler.RaiseHorizontalRulerMoveEvent(e);
        verticalRuler.RaiseVerticalRulerMoveEvent(e);
    }

    public void SetViewModel(SnapScreen snapScreen, Layout layout)
    {
        var model = ViewModel; //DataContext as DesignWindowViewModel;
        if (model != null)
        {
            model.Window = this;
            model.SnapScreen = snapScreen;
            model.Layout = layout;

            designerSizeText.Text = $"Designer Size     {snapScreen.Bounds.Width:0.00} x {snapScreen.Bounds.Height:0.00}";
        }

        if (Dev.IsTopmostDisabled)
        {
            Topmost = false;
        }
    }
}