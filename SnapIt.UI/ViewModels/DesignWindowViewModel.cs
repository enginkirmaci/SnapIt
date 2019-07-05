using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Entities;

namespace SnapIt.UI.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        private readonly SolidColorBrush backgroundBrush = new SolidColorBrush(Color.FromArgb(100, 150, 150, 150));

        public Grid MainSnapArea { get; set; }
        public DelegateCommand<object> LoadedCommand { get; }

        public DesignWindowViewModel()
        {
            LoadedCommand = new DelegateCommand<object>((mainSnapArea) =>
            {
                MainSnapArea = mainSnapArea as Grid;

                SplitVertically(MainSnapArea);
                SplitVertically(_leftArea.SnapAreaGrid);
                SplitHorizontally(_rightArea.SnapAreaGrid);
            });
        }

        private SnapArea _leftArea;
        private SnapArea _rightArea;

        private void SplitVertically(Grid snapAreaGrid)
        {
            snapAreaGrid.ColumnDefinitions.Add(new ColumnDefinition());
            snapAreaGrid.ColumnDefinitions.Add(new ColumnDefinition());

            var leftArea = new SnapArea();
            var rightArea = new SnapArea();

            snapAreaGrid.Children.Add(leftArea);
            snapAreaGrid.Children.Add(rightArea);

            Grid.SetColumn(rightArea, 1);

            var splitter = new GridSplitter
            {
                Width = 10,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                ResizeDirection = GridResizeDirection.Auto,
                Background = backgroundBrush
            };

            snapAreaGrid.Children.Add(splitter);

            Grid.SetColumn(splitter, 1);

            //todo test
            _leftArea = leftArea;
            _rightArea = rightArea;
        }

        private void SplitHorizontally(Grid snapAreaGrid)
        {
            snapAreaGrid.RowDefinitions.Add(new RowDefinition());
            snapAreaGrid.RowDefinitions.Add(new RowDefinition());

            var topArea = new SnapArea();
            var bottomArea = new SnapArea();

            snapAreaGrid.Children.Add(topArea);
            snapAreaGrid.Children.Add(bottomArea);

            Grid.SetRow(bottomArea, 1);

            var splitter = new GridSplitter
            {
                Height = 10,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                ResizeDirection = GridResizeDirection.Auto,
                Background = backgroundBrush
            };

            snapAreaGrid.Children.Add(splitter);

            Grid.SetRow(splitter, 1);
        }
    }
}