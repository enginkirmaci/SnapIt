using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public bool _isInDrag = false;
        public Dictionary<object, TranslateTransform> PointDict = new Dictionary<object, TranslateTransform>();
        public Point _anchorPoint;
        public Point _currentPoint;

        public Window1()
        {
            InitializeComponent();
        }

        public void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isInDrag)
            {
                var element = sender as FrameworkElement;
                element.ReleaseMouseCapture();
                Panel.SetZIndex(element, 0);
                _isInDrag = false;
                e.Handled = true;
            }
        }

        public void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            _anchorPoint = e.GetPosition(null);
            element.CaptureMouse();
            Panel.SetZIndex(element, 10);
            _isInDrag = true;
            e.Handled = true;
        }

        public void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isInDrag)
            {
                _currentPoint = e.GetPosition(null);
                if (sender is FrameworkElement fw)
                {
                    if (fw.Parent is FrameworkElement fwParent)
                    {
                        var canvasLeft = Canvas.GetLeft(sender as UIElement);
                        var canvasTop = Canvas.GetTop(sender as UIElement);
                        var p = new Point(_currentPoint.X - _anchorPoint.X + canvasLeft,
                                            _currentPoint.Y - _anchorPoint.Y + canvasTop);
                        var testResults = new List<HitTestResult>()
                        {
                            VisualTreeHelper.HitTest(a3  , p),
                            VisualTreeHelper.HitTest(a3, new Point(p.X + fw.Width, p.Y)),
                            VisualTreeHelper.HitTest(a3, new Point(p.X, p.Y + fw.Height)),
                            VisualTreeHelper.HitTest(a3, new Point(p.X + fw.Width, p.Y +fw.Height)),
                        };
                        var success = true;
                        foreach (var testResult in testResults)
                        {
                            if (testResult != null)
                            {
                                if (testResult.VisualHit != sender && testResult.VisualHit != fwParent && fw.IsAncestorOf(testResult.VisualHit) == false)
                                {
                                    success = false;
                                    break;
                                }
                            }
                        }
                        if (success)
                        {
                            if (!double.IsNaN(canvasTop))
                            {
                                Canvas.SetTop(sender as UIElement, p.Y);
                            }
                            if (double.IsNaN(canvasLeft))
                            {
                                Canvas.SetLeft(sender as UIElement, p.X);
                            }
                            _anchorPoint = _currentPoint;
                        }
                    }
                }
            }
        }
    }
}