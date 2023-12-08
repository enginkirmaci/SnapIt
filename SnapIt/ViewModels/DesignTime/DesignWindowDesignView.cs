using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;
using Point = SnapIt.Common.Graphics.Point;
using Size = SnapIt.Common.Graphics.Size;

namespace SnapIt.ViewModels.DesignTime;

public class DesignWindowDesignView
{
    public Layout Layout { get; set; }
    public SnapAreaTheme Theme { get; set; }

    public DesignWindowDesignView()
    {
        Theme = new SnapAreaTheme
        {
            HighlightColor = System.Windows.Media.Color.FromArgb(200, 33, 33, 33),
            OverlayColor = System.Windows.Media.Color.FromArgb(200, 99, 99, 99),
            BorderColor = System.Windows.Media.Color.FromArgb(150, 200, 200, 200),
            BorderThickness = 1,
            Opacity = 1
        };

        Layout = new Layout
        {
            Name = "Layout 1",
            Size = new Size(1436, 700.8f),
            LayoutLines = new List<Line>
            {
                new Line
                {
                    Point=new Point(259.904414003044f,0),
                    Size = new Size(0,700.8f)
                },
                 new Line
                {
                    Point=new Point( 1230.4596651446f,0),
                    Size = new Size(0,700.8f )
                },
                 new Line
                {
                    Point=new Point(259.904414003044f,471.072897196262f ),
                    Size = new Size(970.555251141553f,0 ),
                    SplitDirection = SplitDirection.Horizontal
                },
                 new Line
                {
                    Point=new Point(455.331811263318f,471.072897196262f ),
                    Size = new Size(0,229.727102803738f )
                },
                 new Line
                {
                    Point=new Point(567.733637747336f,0 ),
                    Size = new Size( 0,471.072897196262f)
                },
                 new Line
                {
                    Point=new Point(567,235.5f ),
                    Size = new Size(663,0 ),
                    SplitDirection = SplitDirection.Horizontal
                },
                 new Line
                {
                    Point=new Point(898.5f,235 ),
                    Size = new Size( 0,236)
                },
                 new Line
                {
                    Point=new Point(842.5f,471 ),
                    Size = new Size( 0,229)
                }
            },
            Theme = Theme
        };
    }
}