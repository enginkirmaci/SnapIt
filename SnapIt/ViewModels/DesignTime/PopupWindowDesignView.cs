using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels.DesignTime
{
    public class PopupWindowDesignView
    {
        public SnapAreaTheme Theme { get; set; } = new SnapAreaTheme();
        public ObservableCollection<SnapScreen> SnapScreens { get; set; }

        public SnapScreen SelectedSnapScreen { get; set; }
        public ApplicationGroup SelectedApplicationGroup { get; set; }

        public PopupWindowDesignView()
        {
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

            SnapScreens = new ObservableCollection<SnapScreen>
            {
                new SnapScreen( )
                {
                    DeviceNumber = "1",
                    Primary = "Primary",
                    Resolution = "1920 x 1080",
                    Layout = new Layout
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
                    WorkingArea = first,
                    Bounds = first,
                    IsActive = false
                },
                new SnapScreen() {
                    DeviceNumber = "2",
                    IsActive = true,
                    Primary = null,
                    Resolution = "3440 x 1440",
                    Layout =  new Layout
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
                    WorkingArea = second,
                    Bounds = second
                }
            };

            SelectedSnapScreen = SnapScreens.First();

            SelectedSnapScreen.ApplicationGroups = new List<ApplicationGroup>
            {
                new ApplicationGroup
                {
                    Name = "Start Day",
                    ApplicationAreas = new ObservableCollectionWithItemNotify<ApplicationArea>
                    {
                        new ApplicationArea(1)
                        {
                            Applications = new ObservableCollectionWithItemNotify<ApplicationItem>
                            {
                                new ApplicationItem
                                {
                                    Path ="Test 1"
                                },
                                new ApplicationItem
                                {
                                    Path ="Test 2"
                                }
                            }
                        }
                    }
                },
                new ApplicationGroup
                {
                    Name = "Work",
                    ApplicationAreas = new ObservableCollectionWithItemNotify<ApplicationArea>
                    {
                        new ApplicationArea(1)
                        {
                            Applications = new ObservableCollectionWithItemNotify<ApplicationItem>
                            {
                                new ApplicationItem
                                {
                                    AreaNumber =4,
                                    Path ="Test 1"
                                },
                                new ApplicationItem
                                {
                                    Path ="Test 2"
                                }
                            }
                        },
                        new ApplicationArea(2)
                        {
                            Applications = new ObservableCollectionWithItemNotify<ApplicationItem>
                            {
                                new ApplicationItem
                                {
                                    Path ="Test 1"
                                },
                                new ApplicationItem
                                {
                                    Path ="Test 2"
                                },
                                new ApplicationItem
                                {
                                    Path ="Test 3"
                                }
                            }
                        }
                    }
                }
            };

            SelectedApplicationGroup = SelectedSnapScreen.ApplicationGroups.FirstOrDefault();
        }
    }
}