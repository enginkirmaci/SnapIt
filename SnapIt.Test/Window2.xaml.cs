using System.Collections.Generic;
using System.Windows;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public List<string> Items { get; set; }

        public Window2()
        {
            InitializeComponent();

            Items = new List<string>();
            Items.Add("t");
            Items.Add("t");
            Items.Add("t");
            Items.Add("t");
        }
    }
}