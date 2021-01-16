using System.Collections.Generic;
using System.Windows;
using SnapIt.Library.Entities;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            SnapControl.Layout = new Layout();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            SnapControl.Layout = new Layout
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
                Theme = new SnapAreaTheme()
            };
        }

        private void AddOverlay_Click(object sender, RoutedEventArgs e)
        {
            SnapControl.AddOverlay();
        }
    }
}