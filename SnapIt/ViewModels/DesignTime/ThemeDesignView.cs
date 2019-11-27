using System.Collections.Generic;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels.DesignTime
{
    public class ThemeDesignView
    {
        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get; set; }

        public ThemeDesignView()
        {
            Theme = new SnapAreaTheme();

            Layout = new Layout
            {
                Name = "Layout 1",
                LayoutArea = new LayoutArea
                {
                    Areas = new List<LayoutArea>
                    {
                        new LayoutArea
                        {
                            Width=1
                        },
                        new LayoutArea
                        {
                            Width=3,
                            Column=1,
                            Merged= true,
                            Areas = new List<LayoutArea>
                            {
                                new LayoutArea
                                {
                                    Height=1
                                },
                                new LayoutArea
                                {
                                    Height=1,
                                    Row=1,
                                    Areas = new List<LayoutArea>
                                    {
                                        new LayoutArea
                                        {
                                            Width=1
                                        },
                                        new LayoutArea
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