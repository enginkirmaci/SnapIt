using System;
using System.Windows;
using System.Windows.Interop;
using SnapIt.Controls;
using SnapIt.UI.ViewModels;

namespace SnapIt.UI.Views
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : Window
    {
        private SnapScreen snapScreen;

        public DesignWindow()
        {
            InitializeComponent();
        }

        public void SetScreen(SnapScreen snapScreen, Entities.Layout layout)
        {
            this.snapScreen = snapScreen;

            Width = snapScreen.Base.WorkingArea.Width;
            Height = snapScreen.Base.WorkingArea.Height;
            Left = snapScreen.Base.WorkingArea.X;
            Top = snapScreen.Base.WorkingArea.Y;

            var model = DataContext as DesignWindowViewModel;
            model.SnapScreen = snapScreen;
            model.Layout = layout;

            if (DevMode.IsActive)
            {
                Topmost = false;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var wih = new WindowInteropHelper(this);
            IntPtr hWnd = wih.Handle;

            User32Test.MoveWindow(hWnd,
                                  snapScreen.Base.WorkingArea.Left,
                                  snapScreen.Base.WorkingArea.Top,
                                  snapScreen.Base.WorkingArea.Width,
                                  snapScreen.Base.WorkingArea.Height);
        }
    }
}