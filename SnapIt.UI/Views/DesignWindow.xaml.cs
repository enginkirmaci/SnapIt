using System.Windows;
using System.Windows.Forms;

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

        //public void SetScreen(Screen screen)
        //{
        //    this.screen = screen;

        //    Topmost = true;
        //    AllowsTransparency = true;
        //    Background = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
        //    ResizeMode = ResizeMode.NoResize;
        //    ShowInTaskbar = false;
        //    Width = screen.WorkingArea.Width;
        //    Height = screen.WorkingArea.Height;
        //    Left = screen.WorkingArea.X;
        //    Top = screen.WorkingArea.Y;
        //    WindowState = WindowState.Normal;
        //    WindowStyle = WindowStyle.None;
        //}

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);

        //    var wih = new WindowInteropHelper(this);
        //    IntPtr hWnd = wih.Handle;

        //    User32Test.MoveWindow(hWnd, screen.WorkingArea.Left, screen.WorkingArea.Top, screen.WorkingArea.Width, screen.WorkingArea.Height);
        //}
    }
}