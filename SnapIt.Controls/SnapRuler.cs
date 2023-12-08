using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SnapIt.Controls;

public enum enumOrientation
{ Horizontal, Vertical }

public class SnapRuler : Control
{
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register("Orientation", typeof(enumOrientation), typeof(SnapRuler),
        new UIPropertyMetadata(enumOrientation.Horizontal));

    public enumOrientation Orientation
    {
        get => (enumOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty MajorIntervalProperty =
        DependencyProperty.Register("MajorIntervalProperty", typeof(int), typeof(SnapRuler),
        new UIPropertyMetadata(100));

    public int MajorInterval
    {
        get => (int)GetValue(MajorIntervalProperty);
        set => SetValue(MajorIntervalProperty, value);
    }

    public static readonly DependencyProperty MarkLengthProperty =
        DependencyProperty.Register("MarkLengthProperty", typeof(int), typeof(SnapRuler),
        new UIPropertyMetadata(20));

    public int MarkLength
    {
        get => (int)GetValue(MarkLengthProperty);
        set => SetValue(MarkLengthProperty, value);
    }

    public static readonly DependencyProperty MiddleMarkLengthProperty =
        DependencyProperty.Register("MiddleMarkLengthProperty", typeof(int), typeof(SnapRuler),
        new UIPropertyMetadata(10));

    public int MiddleMarkLength
    {
        get => (int)GetValue(MiddleMarkLengthProperty);
        set => SetValue(MiddleMarkLengthProperty, value);
    }

    public static readonly DependencyProperty LittleMarkLengthProperty =
        DependencyProperty.Register("LittleMarkLengthProperty", typeof(int), typeof(SnapRuler),
        new UIPropertyMetadata(5));

    public int LittleMarkLength
    {
        get => (int)GetValue(LittleMarkLengthProperty);
        set => SetValue(LittleMarkLengthProperty, value);
    }

    public static readonly DependencyProperty StartValueProperty =
        DependencyProperty.Register("StartValueProperty", typeof(double), typeof(SnapRuler),
        new UIPropertyMetadata(0.0));

    public double StartValue
    {
        get => (double)GetValue(StartValueProperty);
        set => SetValue(StartValueProperty, value);
    }

    private readonly Color fontColor;
    private readonly Color majorColor;
    private readonly Color minorColor;
    private Line mouseVerticalTrackLine;
    private Line mouseHorizontalTrackLine;

    public SnapRuler()
    {
        //DefaultStyleKeyProperty.OverrideMetadata(typeof(SnapRuler), new FrameworkPropertyMetadata(typeof(SnapRuler)));

        //fontColor = Color.FromArgb(255, 210, 210, 210);
        fontColor = minorColor = majorColor = Color.FromArgb(255, 255, 255, 255);
        //minorColor = Color.FromArgb(255, 210, 210, 210);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        double psuedoStartValue = StartValue;

        #region Horizontal Ruler

        if (Orientation == enumOrientation.Horizontal)
        {
            for (int i = 0; i < ActualWidth / MajorInterval; i++)
            {
                var ft = new FormattedText((psuedoStartValue * MajorInterval).ToString(), System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, new SolidColorBrush(fontColor), 96);
                drawingContext.DrawText(ft, new Point(1 + i * MajorInterval, LittleMarkLength));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(majorColor), 1), new Point(i * MajorInterval, MarkLength), new Point(i * MajorInterval, 0));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(minorColor), 1),
                    new Point(i * MajorInterval + MajorInterval / 2, MiddleMarkLength),
                    new Point(i * MajorInterval + MajorInterval / 2, 0));
                for (int j = 1; j < 10; j++)
                {
                    if (j == 5)
                    {
                        continue;
                    }
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(minorColor), 1),
                    new Point(i * MajorInterval + MajorInterval * j / 10, LittleMarkLength),
                    new Point(i * MajorInterval + MajorInterval * j / 10, 0));
                }
                psuedoStartValue++;
            }
        }

        #endregion Horizontal Ruler

        #region Vertical Ruler

        else
        {
            psuedoStartValue = StartValue;
            for (int i = 0; i < ActualHeight / MajorInterval; i++)
            {
                var ft = new FormattedText((psuedoStartValue * MajorInterval).ToString(), System.Globalization.CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), 11, new SolidColorBrush(fontColor), 96);
                drawingContext.DrawText(ft, new Point(LittleMarkLength, 1 + i * MajorInterval));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(majorColor), 1), new Point(MarkLength, i * MajorInterval), new Point(0, i * MajorInterval));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(majorColor), 1), new Point(MarkLength, i * MajorInterval), new Point(0, i * MajorInterval));
                drawingContext.DrawLine(new Pen(new SolidColorBrush(minorColor), 1),
                    new Point(MiddleMarkLength, i * MajorInterval + MajorInterval / 2),
                    new Point(0, i * MajorInterval + MajorInterval / 2));
                for (int j = 1; j < 10; j++)
                {
                    if (j == 5)
                    {
                        continue;
                    }
                    drawingContext.DrawLine(new Pen(new SolidColorBrush(minorColor), 1),
                    new Point(LittleMarkLength, i * MajorInterval + MajorInterval * j / 10),
                    new Point(0, i * MajorInterval + MajorInterval * j / 10));
                }
                psuedoStartValue++;
            }
        }

        #endregion Vertical Ruler
    }

    public void RaiseHorizontalRulerMoveEvent(MouseEventArgs e)
    {
        Point mousePoint = e.GetPosition(this);
        mouseHorizontalTrackLine.X1 = mouseHorizontalTrackLine.X2 = mousePoint.X;
    }

    public void RaiseVerticalRulerMoveEvent(MouseEventArgs e)
    {
        Point mousePoint = e.GetPosition(this);
        mouseVerticalTrackLine.Y1 = mouseVerticalTrackLine.Y2 = mousePoint.Y;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        mouseVerticalTrackLine = GetTemplateChild("verticalTrackLine") as Line;
        mouseHorizontalTrackLine = GetTemplateChild("horizontalTrackLine") as Line;
        mouseVerticalTrackLine.Visibility = Visibility.Visible;
        mouseHorizontalTrackLine.Visibility = Visibility.Visible;
    }
}