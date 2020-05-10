using System.Windows;

namespace SnapIt.Test.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static Rect GetRect(this FrameworkElement element)
        {
            return new Rect(new Point(element.Margin.Left, element.Margin.Top), new Size(element.ActualWidth, element.ActualHeight));
        }
    }
}