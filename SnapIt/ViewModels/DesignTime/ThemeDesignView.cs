using System.Collections.Generic;
using System.Windows;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels.DesignTime
{
    public class ThemeDesignView
    {
        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get; set; } = new SnapAreaTheme();
        public bool OpenApplyChangesBar { get; set; }

        public ThemeDesignView()
        {
            OpenApplyChangesBar = true;

            Layout = new Layout
            {
                Name = "Layout 1",
                Size = new Size(500, 200),
                LayoutLines = new List<LayoutLine>
                {
                    new LayoutLine
                    {
                        Point=new Point(150,0),
                        Size = new Size(0,200)
                    },
                     new LayoutLine
                    {
                        Point=new Point(150,100),
                        Size = new Size(350,0),
                        SplitDirection = SplitDirection.Horizontal
                    },
                     new LayoutLine
                    {
                        Point=new Point(325,100),
                        Size = new Size(0,100)
                    }
                },
                Theme = Theme
            };
        }
    }
}