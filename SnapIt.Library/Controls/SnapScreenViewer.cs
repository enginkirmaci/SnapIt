using SnapIt.Library.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SnapIt.Library.Controls
{
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

            if (borders.Count() > 0)
            {
                var snapScreens = (IEnumerable<Entities.SnapScreen>)ItemsSource;

                if (snapScreens.Count() > 0 && ActualWidth != 0)
                {
                    var maxScreenSizeX = snapScreens.Max(screen => screen.Bounds.BottomRight.X);
                    var maxScreenSizeY = snapScreens.Max(screen => screen.Bounds.BottomRight.Y);

                    double factorX, factorY = 0.0;
                    factorX = ActualWidth / maxScreenSizeX;
                    factorY = ActualHeight / maxScreenSizeY;
                    if (factorX > factorY)
                    {
                        factorX = factorY;
                    }
                    else
                    {
                        factorY = factorX;
                    }

                    Width = maxScreenSizeX * factorX;
                    Height = maxScreenSizeY * factorY;

                    foreach (var border in borders)
                    {
                        var snapScreen = (Entities.SnapScreen)border.DataContext;
                        //var snapControl = border.FindChildren<SnapControl>().FirstOrDefault();
                        //if (snapControl != null)
                        //{
                        //    snapControl.Theme = Theme;
                        //}

                        var newPoint = new Point
                        {
                            X = snapScreen.Bounds.X * factorX,
                            Y = snapScreen.Bounds.Y * factorY
                        };
                        var newSize = new Size
                        {
                            Width = snapScreen.Bounds.Width * factorX,
                            Height = snapScreen.Bounds.Height * factorY
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
}