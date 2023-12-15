using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;
using SnapIt.Services;

namespace SnapIt.Test;

/// <summary>
/// Interaction logic for Window2.xaml
/// </summary>
public partial class Window2 : Window
{
    public List<string> Items { get; set; }

    public Window2()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _ = AnimateWindow();
    }

    private async Task AnimateWindow()
    {
        var winApiService = new WinApiService();
        var wih = new WindowInteropHelper(this);
        var window = new ActiveWindow
        {
            Handle = wih.Handle
        };

        double frameCount = 100;

        var from = new Rectangle(300, 300, 600, 600);
        var to = new Rectangle(450, 450, 1000, 1000);

        var pLeft = (to.Left - from.Left) / frameCount;
        var pTop = (to.Top - from.Top) / frameCount;
        var pRight = (to.Right - from.Right) / frameCount;
        var pBottom = (to.Bottom - from.Bottom) / frameCount;

        for (var i = 0; i < frameCount; i++)
        {
            var current = new Rectangle(
                (int)(from.Left + (double)(i * pLeft)),
                (int)(from.Top + (double)(i * pTop)),
                (int)(from.Right + (double)(i * pRight)),
                (int)(from.Bottom + (double)(i * pBottom)));

            //winApiService.MoveWindow(window, current);
            //Dev.Log(current);

            MoveWindow(current, wih.Handle);

            //await Task.Delay((int)(tickDuration * 2));
        }

        winApiService.MoveWindow(window, to);
    }

    private void MoveWindow(Rectangle rectangle, nint handle)
    {
        PInvoke.User32.SetWindowPos(
            handle,
            PInvoke.User32.SpecialWindowHandles.HWND_TOP,
            (int)rectangle.X,
            (int)rectangle.Y,
            (int)rectangle.Width,
            (int)rectangle.Height,
            PInvoke.User32.SetWindowPosFlags.SWP_ASYNCWINDOWPOS | PInvoke.User32.SetWindowPosFlags.SWP_NOSENDCHANGING);
    }
}