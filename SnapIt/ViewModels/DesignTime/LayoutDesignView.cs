using Prism.Commands;
using SnapIt.Library.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace SnapIt.ViewModels.DesignTime
{
    public class LayoutDesignView
    {
        public ObservableCollection<SnapScreen> SnapScreens { get; set; }
        public SnapScreen SelectedSnapScreen { get; set; }
        public ObservableCollection<Layout> Layouts { get; set; }
        public Layout SelectedLayout { get; set; }
        public SnapAreaTheme Theme { get; set; }
        public DelegateCommand DesignLayoutCommand { get; private set; }
        public DelegateCommand ExportLayoutCommand { get; private set; }

        public LayoutDesignView()
        {
            Theme = new SnapAreaTheme
            {
                HighlightColor = Color.FromArgb(200, 33, 33, 33),
                OverlayColor = Color.FromArgb(200, 99, 99, 99),
                BorderColor = Color.FromArgb(150, 200, 200, 200),
                BorderThickness = 1,
                Opacity = 1
            };

            SnapScreens = new ObservableCollection<SnapScreen>();
            SnapScreens.Add(new SnapScreen() { DeviceNumber = "1", Primary = "Primary", Resolution = "1920 x 1080" });
            SnapScreens.Add(new SnapScreen() { DeviceNumber = "2", Primary = null, Resolution = "3440 x 1440" });

            Layouts = new ObservableCollection<Layout>();
            Layouts.Add(new Layout
            {
                Name = "Layout 1",
                Theme = Theme,
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
                            //Merged= true,
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
            });
            Layouts.Add(new Layout
            {
                Name = "Layout 2",
                Theme = Theme,
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
                            //Merged= true,
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
            });
            Layouts.Add(new Layout
            {
                Name = "3 Part Horizontal Reverse",
                Theme = Theme
            });
            Layouts.Add(new Layout
            {
                Name = "Layout 4",
                Theme = Theme
            });

            SelectedLayout = Layouts.First();
            SelectedSnapScreen = SnapScreens.First();
            SnapScreens[0].Layout = Layouts[2];
            SnapScreens[1].Layout = SelectedLayout;
        }
    }

    public class SnapScreen
    {
        public string Primary { get; set; }
        public string DeviceNumber { get; set; }
        public string Resolution { get; set; }
        public Layout Layout { get; set; }
    }
}