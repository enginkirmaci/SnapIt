using System.Windows;
using SnapIt.Library.Controls;
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
            var str = @"{
    'Version': '2.0',
    'Guid': 'a4e1eb3d-376d-473c-afac-cb253cd8ee8e',
    'Name': '2 Part Right Split',
    'Size': '2752,1104',
    'LayoutLines': [
      {
        'Point': '1376,0',
        'Size': '0,1104'
      },
      {
        'Point': '1376,552',
        'Size': '1376,0',
        'SplitDirection': 'Horizontal'
      }
    ],
    'LayoutOverlays': [
      {
        'Point': '1376,0',
        'Size': '1376,1104',
        'MiniOverlay': {
          'Point': '141.60000000000002,71.20000000000002',
          'Size': '412.8,331.2'
        }
      }
    ]
  }";
            var layout = Newtonsoft.Json.JsonConvert.DeserializeObject<Layout>(str);

            SnapControl.Layout = layout;

            //SnapControl.Layout = new Layout
            //{
            //    Name = "Layout 1",
            //    Size = new Size(500, 200),
            //    LayoutLines = new List<LayoutLine>
            //        {
            //            new LayoutLine
            //            {
            //                Point=new Point(150,0),
            //                Size = new Size(0,200)
            //            },
            //             new LayoutLine
            //            {
            //                Point=new Point(150,100),
            //                Size = new Size(350,0),
            //                SplitDirection = SplitDirection.Horizontal
            //            },
            //             new LayoutLine
            //            {
            //                Point=new Point(325,100),
            //                Size = new Size(0,100)
            //            }
            //        },
            //    Theme = new SnapAreaTheme()
            //};
        }

        private void AddOverlay_Click(object sender, RoutedEventArgs e)
        {
            //var c = SnapControl.FindChildren<SnapFullOverlay>();
            //var one = c.FirstOrDefault();
            //one.OnHoverStyle();
            SnapControl.AddOverlay();
        }

        private void HideOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (SnapControl.IsOverlayVisible)
                SnapControl.IsOverlayVisible = false;
            else
                SnapControl.IsOverlayVisible = true;
        }
    }
}