using System.Collections.Generic;
using System.Windows.Media;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels.DesignTime
{
    public class DesignWindowDesignView
    {
        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get; set; }

        public DesignWindowDesignView()
        {
            Theme = new SnapAreaTheme
            {
                HighlightColor = Color.FromArgb(200, 33, 33, 33),
                OverlayColor = Color.FromArgb(200, 99, 99, 99),
                BorderColor = Color.FromArgb(150, 200, 200, 200),
                BorderThickness = 1,
                Opacity = 1
            };

            Layout = new Layout
            {
                Name = "Layout 1",
                Size = new System.Windows.Size(1436, 700.8),
                LayoutLines = new List<LayoutLine>
                {
                    new LayoutLine
                    {
                        Point=new System.Windows.Point(259.904414003044,0),
                        Size = new System.Windows.Size(0,700.8)
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point( 1230.4596651446,0),
                        Size = new System.Windows.Size(0,700.8 )
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point(259.904414003044,471.072897196262 ),
                        Size = new System.Windows.Size(970.555251141553,0 ),
                        SplitDirection = SplitDirection.Horizontal
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point(455.331811263318,471.072897196262 ),
                        Size = new System.Windows.Size(0,229.727102803738 )
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point(567.733637747336,0 ),
                        Size = new System.Windows.Size( 0,471.072897196262)
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point(567,235.5 ),
                        Size = new System.Windows.Size(663,0 ),
                        SplitDirection = SplitDirection.Horizontal
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point(898.5,235 ),
                        Size = new System.Windows.Size( 0,236)
                    },
                     new LayoutLine
                    {
                        Point=new System.Windows.Point(842.5,471 ),
                        Size = new System.Windows.Size( 0,229)
                    }
                },
                Theme = Theme
            };
        }
    }
}