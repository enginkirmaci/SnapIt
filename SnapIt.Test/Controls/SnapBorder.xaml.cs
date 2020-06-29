using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Test.Library;

namespace SnapIt.Test.Controls
{
    /// <summary>
    /// Interaction logic for SnapBorder.xaml
    /// </summary>
    public partial class SnapBorder : UserControl
    {
        public const int THICKNESS = 16;
        public const int THICKNESSHALF = 8;
        public const int MAXSIZE = 40;
        public const int MAXSIZEHALF = 20;

        public SplitDirection SplitDirection { get; set; }
        public bool IsDraggable { get; set; } = true;

        private bool isInDrag = false;
        private Point anchorPoint;

        public SnapBorder()
        {
            InitializeComponent();

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            Border.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
            Border.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            Border.BorderThickness = new Thickness(1);

            ReferenceBorder.Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsDraggable)
            {
                isInDrag = true;

                anchorPoint = e.GetPosition(null);

                CaptureMouse();

                Opacity = 0.6;

                Cursor = Border.Cursor;

                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            StopDragging();
            e.Handled = true;
        }

        private void StopDragging()
        {
            Cursor = Cursors.Arrow;
            isInDrag = false;
            ReleaseMouseCapture();
            Opacity = 1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isInDrag)
            {
                var currentPoint = e.GetPosition(null);

                var p = new Point(currentPoint.X - anchorPoint.X + Margin.Left, currentPoint.Y - anchorPoint.Y + Margin.Top);

                if (!IsCollided(p))
                {
                    if (SplitDirection == SplitDirection.Horizontal)
                    {
                        if (Math.Abs(Margin.Top - p.Y) > THICKNESS - 1)
                        {
                            return;
                        }

                        Margin = new Thickness(Margin.Left, p.Y, 0, 0);
                    }
                    else
                    {
                        if (Math.Abs(Margin.Left - p.X) > THICKNESS - 1)
                        {
                            return;
                        }

                        Margin = new Thickness(p.X, Margin.Top, 0, 0);
                    }

                    anchorPoint = currentPoint;

                    //////////////////////////////////////////////////

                    var thisRect = this.GetRect();

                    thisRect = SplitDirection == SplitDirection.Vertical ?
                        new Rect(
                            thisRect.X + THICKNESS,
                            thisRect.Y + (MAXSIZEHALF - THICKNESSHALF) + 1,
                            ActualWidth + THICKNESS,
                            ActualHeight - (MAXSIZE - THICKNESS) - 2) :
                     new Rect(
                            thisRect.X - (MAXSIZEHALF - THICKNESSHALF),
                            thisRect.Y - THICKNESSHALF,
                            ActualWidth + (MAXSIZEHALF - THICKNESSHALF),
                            ActualHeight - THICKNESS);

                    var splitDirection = SplitDirection == SplitDirection.Horizontal ? SplitDirection.Vertical : SplitDirection.Horizontal;

                    foreach (var col in Parent.FindChildren<SnapBorder>())
                    {
                        col.Border.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                    }

                    var nearBorders = GetCollisions(thisRect, splitDirection);

                    foreach (var col in nearBorders)
                    {
                        col.Border.Background = new SolidColorBrush(Colors.Red);
                    }

                    //foreach (var near in nearBorders.Where(b => b.IsDraggable))
                    //{
                    //    if (SplitDirection == SplitDirection.Horizontal)
                    //    {
                    //        if (Margin.Top > near.Margin.Top) //top
                    //        {
                    //            near.Height = Math.Abs(Margin.Top - near.Margin.Top);
                    //        }
                    //        else //bottom
                    //        {
                    //            var newHeight = Margin.Top - near.Margin.Top + THICKNESS;

                    //            //try catch here
                    //            near.Height -= newHeight;
                    //            near.Margin = new Thickness(near.Margin.Left, near.Margin.Top + newHeight, 0, 0);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (Margin.Left > near.Margin.Left) //left
                    //        {
                    //            near.Width = Math.Abs(Margin.Left - near.Margin.Left);
                    //        }
                    //        else //right
                    //        {
                    //            var newWidth = Margin.Left - near.Margin.Left + THICKNESS;

                    //            //try catch here
                    //            near.Width -= newWidth;
                    //            near.Margin = new Thickness(near.Margin.Left + newWidth, near.Margin.Top, 0, 0);
                    //        }
                    //    }
                    //}
                }
                else
                {
                    StopDragging();
                }
            }
        }

        private bool IsCollided(Point p)
        {
            var isCollided = false;

            var thisRect = SplitDirection == SplitDirection.Vertical ?
                new Rect(
                    p.X + ReferenceBorder.Margin.Left,
                    p.Y + ReferenceBorder.Margin.Top,
                    ReferenceBorder.ActualWidth,
                    ActualHeight) :
                new Rect(
                    p.X + ReferenceBorder.Margin.Left,
                    p.Y + ReferenceBorder.Margin.Top,
                    ActualWidth,
                    ReferenceBorder.ActualHeight);

            foreach (var col in Parent.FindChildren<SnapBorder>())
            {
                col.Border.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
            }

            var collisions = GetCollisions(thisRect, SplitDirection);

            foreach (var col in collisions)
            {
                col.Border.Background = new SolidColorBrush(Colors.Blue);
            }

            foreach (var border in collisions)
            {
                if (border != this && thisRect.IntersectsWith(border.GetRect()))
                {
                    isCollided = true;
                    break;
                }
            }

            return isCollided;
        }

        public void SetPos(Point point, Size size, SplitDirection splitDirection)
        {
            Margin = new Thickness(point.X, point.Y, 0, 0);
            SplitDirection = splitDirection;

            if (IsDraggable)
            {
                if (SplitDirection == SplitDirection.Horizontal)
                {
                    Width = size.Width;
                    Height = MAXSIZE;

                    Margin = new Thickness(Margin.Left, Margin.Top - MAXSIZEHALF, 0, 0);

                    Border.Cursor = Cursors.SizeNS;
                    Border.Height = THICKNESS;
                    Border.HorizontalAlignment = HorizontalAlignment.Stretch;

                    ReferenceBorder.Height = 1;
                    ReferenceBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(0, MAXSIZEHALF, 0, 0);
                }
                else
                {
                    Width = MAXSIZE;
                    Height = size.Height;

                    Margin = new Thickness(Margin.Left - MAXSIZEHALF, Margin.Top, 0, 0);

                    Border.Cursor = Cursors.SizeWE;
                    Border.Width = THICKNESS;
                    Border.VerticalAlignment = VerticalAlignment.Stretch;

                    ReferenceBorder.Width = 1;
                    ReferenceBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(MAXSIZEHALF, 0, 0, 0);
                }
            }
            else
            {
                if (SplitDirection == SplitDirection.Horizontal)
                {
                    Width = size.Width;
                    Height = THICKNESS;

                    Border.Height = THICKNESS;
                    Border.HorizontalAlignment = HorizontalAlignment.Stretch;

                    ReferenceBorder.Height = 1;
                    ReferenceBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(0, ActualHeight / 2, 0, 0);
                }
                else
                {
                    Width = THICKNESS;
                    Height = size.Height;

                    Border.Width = THICKNESS;
                    Border.VerticalAlignment = VerticalAlignment.Stretch;

                    ReferenceBorder.Width = 1;
                    ReferenceBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(ActualWidth / 2, 0, 0, 0);
                }

                BorderControls.Visibility = Visibility.Collapsed;
            }
        }

        public SnapLine GetLine()
        {
            return SplitDirection == SplitDirection.Horizontal ?
                new SnapLine
                {
                    Start = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left, Margin.Top + ReferenceBorder.Margin.Top),
                    End = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left + Width, Margin.Top + ReferenceBorder.Margin.Top)
                } :
                new SnapLine
                {
                    Start = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left, Margin.Top + ReferenceBorder.Margin.Top),
                    End = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left, Margin.Top + ReferenceBorder.Margin.Top + Height)
                };
        }

        private List<SnapBorder> GetCollisions(Rect rect, SplitDirection splitDirection)
        {
            var borders = Parent.FindChildren<SnapBorder>();

            var result = borders
                .Where(b => b.SplitDirection == splitDirection && rect.IntersectsWith(b.GetRect()))
                .ToList();

            result.Remove(this);

            return result;
        }

        public Rect GetRect()
        {
            return SplitDirection == SplitDirection.Vertical ?
                           new Rect(
                               Margin.Left + (MAXSIZEHALF - THICKNESSHALF),
                               Margin.Top,
                               THICKNESS,
                               ActualHeight) :
                       new Rect(
                               Margin.Left,
                               Margin.Top + (MAXSIZEHALF - THICKNESSHALF),
                               ActualWidth,
                               THICKNESS);
        }
    }
}