using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Test.Controls
{
    public class SnapBorder : Border
    {
        private const int THICKNESS = 16;

        public SplitDirection SplitDirection { get; set; }
        public bool IsDraggable { get; set; } = true;

        private bool isInDrag = false;
        private Point anchorPoint;

        public SnapBorder()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
            BorderThickness = new Thickness(1);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (IsDraggable)
            {
                if (SplitDirection == SplitDirection.Horizontally)
                {
                    Cursor = Cursors.SizeNS;
                    Height = THICKNESS;
                }
                else
                {
                    Cursor = Cursors.SizeWE;
                    Width = THICKNESS;
                }
            }
            else
            {
                BorderThickness = new Thickness(1, 1, 0, 0);
            }
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
                    if (SplitDirection == SplitDirection.Horizontally)
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

                    //var borders = Parent.FindChildren<SnapBorder>();
                    //foreach (var border in borders)
                    //{
                    //    border.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                    //}

                    var thisRect = GetRect();

                    thisRect = SplitDirection == SplitDirection.Vertically ?
                        new Rect(thisRect.X - THICKNESS, thisRect.Y + 1, ActualWidth + THICKNESS * 2, ActualHeight - 2) :
                        new Rect(thisRect.X + 1, thisRect.Y - THICKNESS, ActualWidth - 2, ActualHeight + THICKNESS * 2);
                    var splitDirection = SplitDirection == SplitDirection.Horizontally ? SplitDirection.Vertically : SplitDirection.Horizontally;

                    var nearBorders = GetCollisions(thisRect, splitDirection);
                    foreach (var near in nearBorders.Where(b => b.IsDraggable))
                    {
                        //near.Background = new SolidColorBrush(Colors.Red);

                        if (SplitDirection == SplitDirection.Horizontally)
                        {
                            if (Margin.Top > near.Margin.Top) //top
                            {
                                near.Height = Math.Abs(Margin.Top - near.Margin.Top);
                            }
                            else //bottom
                            {
                                var newHeight = Margin.Top - near.Margin.Top + THICKNESS;

                                near.Height -= newHeight;
                                near.Margin = new Thickness(near.Margin.Left, near.Margin.Top + newHeight, 0, 0);
                            }
                        }
                        else
                        {
                            if (Margin.Left > near.Margin.Left) //left
                            {
                                near.Width = Math.Abs(Margin.Left - near.Margin.Left);
                            }
                            else //right
                            {
                                var newWidth = Margin.Left - near.Margin.Left + THICKNESS;

                                near.Width -= newWidth;
                                near.Margin = new Thickness(near.Margin.Left + newWidth, near.Margin.Top, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    StopDragging();
                }
            }
        }

        public Library.Line GetLine()
        {
            return SplitDirection == SplitDirection.Horizontally ?
                new Library.Line
                {
                    Start = new Library.Point(Margin.Left, Margin.Top),
                    End = new Library.Point(Margin.Left + ActualWidth, Margin.Top)
                } :
                new Library.Line
                {
                    Start = new Library.Point(Margin.Left, Margin.Top),
                    End = new Library.Point(Margin.Left, Margin.Top + ActualHeight)
                };
        }

        private bool IsCollided(Point p)
        {
            var isCollided = false;

            var thisRect = new Rect(p.X, p.Y, ActualWidth, ActualHeight);

            foreach (var border in GetCollisions(thisRect, SplitDirection))
            {
                if (border != this && thisRect.IntersectsWith(border.GetRect()))
                {
                    isCollided = true;
                    break;
                }
            }

            return isCollided;
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

        private Rect GetRect()
        {
            return new Rect(new Point(Margin.Left, Margin.Top), new Size(ActualWidth, ActualHeight));
        }
    }
}