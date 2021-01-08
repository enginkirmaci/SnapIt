using SnapIt.Library.Entities;
using System.Collections.Generic;
using System.Windows.Media;

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
                LayoutArea = new LayoutAreaOld
                {
                    Areas = new List<LayoutAreaOld>
                    {
                        new LayoutAreaOld
                        {
                            Width=1
                        },
                        new LayoutAreaOld
                        {
                            Width=3,
                            Column=1,
                            //Merged= true,
                            Areas = new List<LayoutAreaOld>
                            {
                                new LayoutAreaOld
                                {
                                    Height=1
                                },
                                new LayoutAreaOld
                                {
                                    Height=1,
                                    Row=1,
                                    Areas = new List<LayoutAreaOld>
                                    {
                                        new LayoutAreaOld
                                        {
                                            Width=1
                                        },
                                        new LayoutAreaOld
                                        {
                                            Width=1,
                                            Column=1
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}