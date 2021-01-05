using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Library.Controls
{
    /// <summary>
    /// Interaction logic for SnapBorder.xaml
    /// </summary>
    public partial class SnapBorder : UserControl
    {
        public const int THICKNESS = 16;
        public const int THICKNESSHALF = 8;
        //public const int MAXSIZE = 40;
        //public const int MAXSIZEHALF = 20;

        public SplitDirection SplitDirection { get; set; }
        public bool IsDraggable { get; set; } = true;

        private Point _positionInBlock;

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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsDraggable)
            {
                _positionInBlock = Mouse.GetPosition(this);

                CaptureMouse();

                Opacity = 0.6;

                Cursor = Border.Cursor;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            StopDragging();
        }

        private void StopDragging()
        {
            Cursor = Cursors.Arrow;
            ReleaseMouseCapture();
            Opacity = 1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsMouseCaptured)
            {
                // get the parent container
                var container = VisualTreeHelper.GetParent(this) as UIElement;

                // get the position within the container
                var mousePosition = e.GetPosition(container);

                var p = new Point(mousePosition.X - _positionInBlock.X, mousePosition.Y - _positionInBlock.Y);

                if (!IsCollided(p))
                {
                    var thisRect = this.GetRect();

                    foreach (var col in Parent.FindChildren<SnapBorder>())
                    {
                        col.Border.Background = new SolidColorBrush(Color.FromArgb(255, 60, 60, 60));
                    }

                    var nearBorders = this.GetCollisions(thisRect);

                    foreach (var col in nearBorders)
                    {
                        col.Border.Background = new SolidColorBrush(Colors.Red);
                    }

                    foreach (var near in nearBorders.Where(b => b.IsDraggable))
                    {
                        if (SplitDirection == SplitDirection.Vertical && near.SplitDirection == SplitDirection.Horizontal)
                        {
                            if (Margin.Left >= near.Margin.Left + near.Width - THICKNESS) //left
                            {
                                near.Width = Math.Abs(p.X - near.Margin.Left + THICKNESSHALF);
                            }
                            else if (Margin.Left < near.Margin.Left) //right
                            {
                                var newWidth = p.X - near.Margin.Left + THICKNESSHALF;

                                //try catch here
                                near.Width -= newWidth;
                                near.Margin = new Thickness(near.Margin.Left + newWidth, near.Margin.Top, 0, 0);
                            }
                        }
                        else if (SplitDirection == SplitDirection.Horizontal && near.SplitDirection == SplitDirection.Vertical)
                        {
                            if (Margin.Top >= near.Margin.Top + near.Height - THICKNESS) //top
                            {
                                near.Height = Math.Abs(p.Y - near.Margin.Top + THICKNESSHALF);
                            }
                            else if (Margin.Top < near.Margin.Top)  //bottom
                            {
                                var newHeight = p.Y - near.Margin.Top + THICKNESSHALF;

                                //try catch here
                                near.Height -= newHeight;
                                near.Margin = new Thickness(near.Margin.Left, near.Margin.Top + newHeight, 0, 0);
                            }
                        }
                    }

                    //////////////////////////////////////////////////

                    if (SplitDirection == SplitDirection.Horizontal)
                    {
                        Margin = new Thickness(Margin.Left, p.Y, 0, 0);
                    }
                    else
                    {
                        Margin = new Thickness(p.X, Margin.Top, 0, 0);
                    }
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
                    p.X,
                    Margin.Top + 1,
                    ActualWidth,
                    ActualHeight - 2) :
                new Rect(
                    Margin.Left + 1,
                    p.Y,
                    ActualWidth - 2,
                    ActualHeight);

            var collisions = this.GetCollisions(thisRect, SplitDirection);

            foreach (var border in collisions)
            {
                if (border != this && thisRect.IntersectsWith(border.GetRect()))
                {
                    border.Border.Background = new SolidColorBrush(Colors.Blue);

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
                if (SplitDirection == SplitDirection.Vertical)
                {
                    Width = THICKNESS;
                    Height = size.Height;

                    Margin = new Thickness(Margin.Left - THICKNESSHALF, Margin.Top, 0, 0);

                    Border.Cursor = Cursors.SizeWE;
                    Border.Width = THICKNESS;
                    Border.VerticalAlignment = VerticalAlignment.Stretch;

                    ReferenceBorder.Width = 1;
                    ReferenceBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(THICKNESSHALF, 0, 0, 0);
                }
                else
                {
                    Width = size.Width;
                    Height = THICKNESS;

                    Margin = new Thickness(Margin.Left, Margin.Top - THICKNESSHALF, 0, 0);

                    Border.Cursor = Cursors.SizeNS;
                    Border.Height = THICKNESS;
                    Border.HorizontalAlignment = HorizontalAlignment.Stretch;

                    ReferenceBorder.Height = 1;
                    ReferenceBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(0, THICKNESSHALF, 0, 0);
                }
            }
            else
            {
                if (SplitDirection == SplitDirection.Vertical)
                {
                    Width = THICKNESS;
                    Height = size.Height;

                    Border.Width = THICKNESS;
                    Border.VerticalAlignment = VerticalAlignment.Stretch;

                    ReferenceBorder.Width = 1;
                    ReferenceBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(ActualWidth / 2, 0, 0, 0);
                }
                else
                {
                    Width = size.Width;
                    Height = THICKNESS;

                    Border.Height = THICKNESS;
                    Border.HorizontalAlignment = HorizontalAlignment.Stretch;

                    ReferenceBorder.Height = 1;
                    ReferenceBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
                    ReferenceBorder.Margin = new Thickness(0, ActualHeight / 2, 0, 0);
                }

                //BorderControls.Visibility = Visibility.Collapsed;
            }
        }

        public SnapLine GetLine()
        {
            return SplitDirection == SplitDirection.Vertical ?
                new SnapLine
                {
                    Start = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left, Margin.Top + ReferenceBorder.Margin.Top),
                    End = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left, Margin.Top + ReferenceBorder.Margin.Top + Height)
                } :
                new SnapLine
                {
                    Start = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left, Margin.Top + ReferenceBorder.Margin.Top),
                    End = new SnapPoint(Margin.Left + ReferenceBorder.Margin.Left + Width, Margin.Top + ReferenceBorder.Margin.Top)
                };
        }
    }
}