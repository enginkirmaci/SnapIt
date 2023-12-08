using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace SnapIt.Controls;

public class SnapScreenViewer : ListView
{
    //public SnapAreaTheme Theme { get; set; } = new SnapAreaTheme();

    public SnapScreenViewer()
    {
        SizeChanged += SnapScreenViewer_SizeChanged;
    }

    protected override void OnChildDesiredSizeChanged(UIElement child)
    {
        base.OnChildDesiredSizeChanged(child);
        AdoptToScreen();
    }

    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);
    }

    private void SnapScreenViewer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AdoptToScreen();
    }

    private void AdoptToScreen()
    {
        var borders = this.FindChildren<Border>("ItemBorder");

        if (borders.Any())
        {
            var snapScreens = (IEnumerable<SnapScreen>)ItemsSource;

            if (snapScreens.Any() && ActualWidth != 0)
            {
                var maxScreenSizeX = snapScreens.Max(screen => screen.WorkingArea.BottomRight.X);
                var maxScreenSizeY = snapScreens.Max(screen => screen.WorkingArea.BottomRight.Y);

                var minScreenSizeX = Math.Abs(snapScreens.Min(screen => screen.WorkingArea.TopLeft.X));
                var minScreenSizeY = Math.Abs(snapScreens.Min(screen => screen.WorkingArea.TopLeft.Y));

                double factorX, factorY = 0.0;
                factorX = ActualWidth / (maxScreenSizeX + minScreenSizeX);
                factorY = ActualHeight / (maxScreenSizeY + minScreenSizeY);
                if (factorX > factorY)
                {
                    factorX = factorY;
                }
                else
                {
                    factorY = factorX;
                }

                Width = (maxScreenSizeX + minScreenSizeX) * factorX;
                Height = (maxScreenSizeY + minScreenSizeY) * factorY;

                foreach (var border in borders)
                {
                    var snapScreen = (SnapScreen)border.DataContext;
                    //var snapControl = border.FindChildren<SnapControl>().FirstOrDefault();
                    //if (snapControl != null)
                    //{
                    //    snapControl.Theme = Theme;
                    //}

                    var newPoint = new Point
                    {
                        X = (snapScreen.WorkingArea.X + minScreenSizeX) * factorX,
                        Y = (snapScreen.WorkingArea.Y + minScreenSizeY) * factorY
                    };
                    var newSize = new Size
                    {
                        Width = snapScreen.WorkingArea.Width * factorX,
                        Height = snapScreen.WorkingArea.Height * factorY
                    };

                    SetPos(border, newPoint, newSize);
                }
            }
        }
    }

    public void SetPos(Border border, Point point, Size size)
    {
        if (!point.X.Equals(double.NaN) && !point.Y.Equals(double.NaN))
        {
            border.Margin = new Thickness(point.X, point.Y, 0, 0);

            border.Width = size.Width;
            border.Height = size.Height;
        }
    }
}