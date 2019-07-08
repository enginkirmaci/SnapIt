using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace SnapIt.UI.Views
{
    /// <summary>
    /// Interaction logic for DesignWindow.xaml
    /// </summary>
    public partial class DesignWindow : Window
    {
        private Screen screen;

        public DesignWindow()
        {
            InitializeComponent();
        }

        public void SetScreen(Screen screen)
        {
            this.screen = screen;

            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Left = screen.WorkingArea.X;
            Top = screen.WorkingArea.Y;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var wih = new WindowInteropHelper(this);
            IntPtr hWnd = wih.Handle;

            User32Test.MoveWindow(hWnd, screen.WorkingArea.Left, screen.WorkingArea.Top, screen.WorkingArea.Width, screen.WorkingArea.Height);
        }
    }
}