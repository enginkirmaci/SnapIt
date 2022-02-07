using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Library.Controls
{
    public class SnapBorderTool : Control
    {
        private const decimal STEP = 8;
        public SnapBorder SnapBorder { get; set; }

        public SplitDirection SplitDirection { get => SnapBorder.SplitDirection; }

        public string PositionX
        {
            get => (string)GetValue(PositionXProperty);
            set => SetValue(PositionXProperty, value);
        }

        public static readonly DependencyProperty PositionXProperty = DependencyProperty.Register(nameof(PositionX),
            typeof(string), typeof(SnapBorderTool), new PropertyMetadata(null));

        //private static void PositionXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var SnapBorderTool = (SnapBorderTool)d;

        //    if (e.OldValue != null && double.TryParse(e.NewValue as string, out var positionX))
        //    {
        //        SnapBorderTool.SnapBorder.MoveBorder(new Point(positionX, SnapBorderTool.SnapBorder.ReferenceBorder.Margin.Top));
        //    }
        //}

        public string PositionY
        {
            get => (string)GetValue(PositionYProperty);
            set => SetValue(PositionYProperty, value);
        }

        public static readonly DependencyProperty PositionYProperty = DependencyProperty.Register(nameof(PositionY),
            typeof(string), typeof(SnapBorderTool), new PropertyMetadata(null));

        public static readonly DependencyProperty ApplyCommandProperty =
        DependencyProperty.Register("ApplyCommand",
            typeof(WPFUI.Common.RelayCommand), typeof(SnapAreaEditor), new PropertyMetadata(null));

        public WPFUI.Common.RelayCommand ApplyCommand => (WPFUI.Common.RelayCommand)GetValue(ApplyCommandProperty);

        public SnapBorderTool()
        {
            SetValue(ApplyCommandProperty,
                new WPFUI.Common.RelayCommand(o =>
                {
                    if (double.TryParse(PositionX, out var positionX) && double.TryParse(PositionY, out var positionY))
                    {
                        if (positionX != 0)
                        {
                            positionX -= 6;
                        }
                        if (positionY != 0)
                        {
                            positionY -= 6;
                        }
                        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
                        {
                            var inProgress = true;

                            decimal delta = decimal.Zero;
                            decimal deltaX = decimal.Zero;
                            decimal deltaY = decimal.Zero;

                            var borderRect = SnapBorder.GetRect();
                            var previousRect = borderRect;

                            if (SplitDirection == SplitDirection.Horizontal)
                            {
                                delta = (decimal)positionY - (decimal)borderRect.Top;
                                deltaY = delta % STEP;

                                if (deltaY == 0)
                                {
                                    deltaY = delta > 0 ? STEP : -STEP;
                                }
                            }
                            else
                            {
                                delta = (decimal)positionX - (decimal)borderRect.Left;
                                deltaX = delta % STEP;

                                if (deltaX == 0)
                                {
                                    deltaX = delta > 0 ? STEP : -STEP;
                                }
                            }

                            while (inProgress && Math.Abs(delta) > decimal.Zero)
                            {
                                DevMode.Log($"delta: {delta}, X: {deltaX}, Y: {deltaY}");

                                previousRect = borderRect;

                                inProgress = SnapBorder.MoveBorder(new Point((double)((decimal)borderRect.Left + deltaX), (double)((decimal)borderRect.Top + deltaY)), false);

                                borderRect = SnapBorder.GetRect();

                                if (borderRect == previousRect)
                                    break;

                                if (SplitDirection == SplitDirection.Horizontal)
                                {
                                    delta = (decimal)positionY - (decimal)borderRect.Top;
                                    deltaY = delta % STEP;

                                    if (deltaY == 0)
                                    {
                                        deltaY = delta > 0 ? STEP : -STEP;
                                    }
                                }
                                else
                                {
                                    delta = (decimal)positionX - (decimal)borderRect.Left;
                                    deltaX = delta % STEP;

                                    if (deltaX == 0)
                                    {
                                        deltaX = delta > 0 ? STEP : -STEP;
                                    }
                                }
                            }

                            SnapBorder.SnapControl.GenerateSnapAreas();

                            var borders = SnapBorder.SnapControl.FindChildren<SnapBorder>();
                            foreach (var border in borders.Where(b => b.IsDraggable))
                            {
                                border?.SnapBorderTool?.ResetPos();
                            }
                        }));
                    }
                }));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //ResetPos();
        }

        public void ResetPos()
        {
            if (SnapBorder != null)
            {
                var borderRect = SnapBorder.GetRect();
                var toolRect = GetRect();
                Margin = new Thickness(
                    borderRect.X + (borderRect.Width / 2) - (toolRect.Width / 2),
                    borderRect.Y + (borderRect.Height / 2) - (toolRect.Height / 2),
                    0, 0);

                PositionX = (borderRect.X + SnapBorder.ReferenceBorder.Margin.Left).ToString("0.00");
                PositionY = (borderRect.Y + SnapBorder.ReferenceBorder.Margin.Top).ToString("0.00");
            }
        }

        public Rect GetRect()
        {
            return new Rect(
                new Point(Margin.Left, Margin.Top),
                new Size(
                    ActualWidth == 0 ? Width : ActualWidth,
                    ActualHeight == 0 ? Height : ActualHeight));
        }
    }
}