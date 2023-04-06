using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Prism.Commands;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels.DesignTime
{
    public class LayoutDesignView
    {
        public ObservableCollection<Library.Entities.SnapScreen> SnapScreens { get; set; }
        public Library.Entities.SnapScreen SelectedSnapScreen { get; set; }
        public ObservableCollection<Layout> Layouts { get; set; }
        public Layout SelectedLayout { get; set; }
        public SnapAreaTheme Theme { get; set; }
        public DelegateCommand DesignLayoutCommand { get; private set; }
        public DelegateCommand ExportLayoutCommand { get; private set; }
        public bool IsRenameDialogOpen { get; set; } = false;

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

            Layouts = new ObservableCollection<Layout>
            {
                new Layout
                {
                    Name = "Layout 1",
                    Size = new System.Windows.Size(500, 200),
                    AreaPadding = 0,
                    LayoutLines = new List<LayoutLine>
                    {
                        new LayoutLine
                        {
                            Point=new System.Windows.Point(150,0),
                            Size = new System.Windows.Size(0,200)
                        },
                         new LayoutLine
                        {
                            Point=new System.Windows.Point(150,100),
                            Size = new System.Windows.Size(350,0),
                            SplitDirection = SplitDirection.Horizontal
                        },
                         new LayoutLine
                        {
                            Point=new System.Windows.Point(325,100),
                            Size = new System.Windows.Size(0,100)
                        }
                    },
                    LayoutOverlays = new List<LayoutOverlay>
                    {
                        new LayoutOverlay
                        {
                            Point= new System.Windows.Point(100,0),
                            Size= new System.Windows.Size(100,100)
                        }
                    },
                    Theme = Theme
                },
                new Layout
                {
                    Name = "Layout 2",
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
                    LayoutOverlays = new List<LayoutOverlay>
                    {
                        new LayoutOverlay
                        {
                            Point= new System.Windows.Point(100,0),
                            Size= new System.Windows.Size(1000,600)
                        }
                    },
                    Theme = Theme
                },
                new Layout
                {
                    Name = "3 Part Horizontal Reverse",
                    Theme = Theme
                },
                new Layout
                {
                    Name = "Layout 4",
                    Theme = Theme
                }
            };

            var first = new System.Windows.Rect
            {
                X = 0,
                Y = 0,
                Width = 300,
                Height = 200
            };
            var second = new System.Windows.Rect
            {
                X = 300,
                Y = 100,
                Width = 100,
                Height = 100
            };

            SnapScreens = new ObservableCollection<Library.Entities.SnapScreen>
            {
                new Library.Entities.SnapScreen( )
                {
                    DeviceNumber = "1",
                    Primary = "Primary",
                    Resolution = "1920 x 1080",
                    Layout = Layouts[0],
                    WorkingArea = first,
                    Bounds = first,
                    IsActive = false
                },
                new Library.Entities.SnapScreen() {
                    DeviceNumber = "2",
                    IsActive = true,
                    Primary = null,
                    Resolution = "3440 x 1440",
                    Layout = Layouts[1],
                    WorkingArea = second,
                    Bounds = second
                }
            };

            SelectedLayout = Layouts.First();
            SelectedSnapScreen = SnapScreens.First();
            SnapScreens[0].Layout = Layouts[1];
            SnapScreens[1].Layout = SelectedLayout;
        }
    }

    //public class SnapScreen : Bindable
    //{
    //    private Layout layout;

    //    public string Primary { get; set; }
    //    public string DeviceNumber { get; set; }
    //    public string Resolution { get; set; }
    //    public bool IsActive { get; set; }

    //    public Layout Layout
    //    { get => layout; set { SetProperty(ref layout, value); } }

    //    public System.Windows.Rect WorkingArea { get; set; }
    //}
}