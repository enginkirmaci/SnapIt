using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.Library.Controls
{
    public class SnapLoadingWindow : Window
    {
        private readonly IWinApiService winApiService;

        public SnapScreen Screen { get; set; }
        public Dpi Dpi { get; set; }

        public static readonly DependencyProperty LoadingMessageProperty =
      DependencyProperty.Register(nameof(LoadingMessage), typeof(string), typeof(SnapLoadingWindow),
          new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string LoadingMessage
        {
            get => (string)GetValue(LoadingMessageProperty);
            set => SetValue(LoadingMessageProperty, value);
        }

        //public string LoadingMessage
        //{
        //    get => (string)GetValue(LoadingMessageProperty);
        //    set => SetValue(LoadingMessageProperty, value);
        //}

        //public static readonly DependencyProperty LoadingMessageProperty
        // = DependencyProperty.Register("LoadingMessageProperty", typeof(string), typeof(SnapLoadingWindow),
        //   new FrameworkPropertyMetadata()
        //   {
        //       DefaultValue = "Test Loading Message",
        //       BindsTwoWayByDefault = true,
        //       PropertyChangedCallback = new PropertyChangedCallback(LoadingMessagePropertyChanged)
        //   });

        //private static void LoadingMessagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var snapLoadingWindow = (SnapLoadingWindow)d;
        //    snapLoadingWindow.LoadingMessage = (string)e.NewValue;
        //}

        public SnapLoadingWindow(
            IWinApiService winApiService,
            SnapScreen screen)
        {
            this.winApiService = winApiService;

            Screen = screen;

            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Left = screen.WorkingArea.X;
            Top = screen.WorkingArea.Y;
            Topmost = true;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;

            Dpi = new Dpi()
            {
                X = 100 / (screen.ScaleFactor * 100),
                Y = 100 / (screen.ScaleFactor * 100)
            };
        }

        public new void Show()
        {
            base.Show();
            MaximizeWindow();
        }

        public void SetLoadingMessage(string message)
        {
            LoadingMessage = message;
        }

        //protected override void OnSourceInitialized(EventArgs e)
        //{
        //    base.OnSourceInitialized(e);

        //    MaximizeWindow();
        //}

        private void MaximizeWindow()
        {
            var wih = new WindowInteropHelper(this);
            var window = new ActiveWindow
            {
                Handle = wih.Handle
            };

            winApiService.MoveWindow(
                window,
                (int)Screen.WorkingArea.Left,
                (int)Screen.WorkingArea.Top,
                (int)Screen.WorkingArea.Width,
                (int)Screen.WorkingArea.Height);
        }
    }
}