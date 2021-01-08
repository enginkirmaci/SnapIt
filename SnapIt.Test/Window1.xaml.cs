using System.Windows;
using SnapIt.Library.Services;

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
            SnapControl.Layout = new Library.Entities.Layout();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SnapControl.SaveLayout();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            FileOperationService fileOperationService = new FileOperationService();
            var layout = fileOperationService.ImportLayout(@"C:\Users\Engin\AppData\Local\SnapIt.Test\Layoutsv20\test.json");

            SnapControl.Layout = layout;
        }
    }
}