using System.Collections.Generic;
using System.Windows;
using SnapIt.Library.Entities;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var Areas = new List<LayoutArea>
            {
                new LayoutArea
                {
                    Column=0,
                    Row=0,
                    ColumnSpan=1,
                    RowSpan=2,
                    Width=1,
                    Height=2
                },
                new LayoutArea
                {
                    Column=1,
                    Row=0,
                    ColumnSpan=2,
                    RowSpan=1,
                    Width=2,
                    Height=1
                },
                new LayoutArea
                {
                    Column=1,
                    Row=1,
                    ColumnSpan=1,
                    RowSpan=1,
                    Width=1,
                    Height=1
                },
                new LayoutArea
                {
                    Column=2,
                    Row=1,
                    ColumnSpan=1,
                    RowSpan=1,
                    Width=1,
                    Height=1
                }
            };

            RootArea.IsDesignMode = true;
            RootArea.Areas = Areas;

            RootArea.ApplyLayout();
        }
    }
}