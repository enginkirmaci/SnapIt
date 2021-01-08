using SnapIt.Library.Entities;
using System.Collections.Generic;

namespace SnapIt.ViewModels.DesignTime
{
    public class ThemeDesignView
    {
        public Layout Layout { get; set; }
        public SnapAreaTheme Theme { get; set; }
        public bool OpenApplyChangesBar { get; set; }

        public ThemeDesignView()
        {
            OpenApplyChangesBar = true;
            Theme = new SnapAreaTheme();

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