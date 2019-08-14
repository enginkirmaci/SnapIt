using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.ViewModels
{
    public class MainWindowDesignView
    {
        public ObservableCollection<SnapScreen> SnapScreens { get; set; }
        public SnapScreen SelectedSnapScreen { get; set; }

        public ObservableCollection<Layout> Layouts { get; set; }
        public Layout SelectedLayout { get; set; }
        public DelegateCommand DesignLayoutCommand { get; private set; }
        public DelegateCommand ExportLayoutCommand { get; private set; }

        public MainWindowDesignView()
        {
            SnapScreens = new ObservableCollection<SnapScreen>();
            SnapScreens.Add(new SnapScreen(null));
            SnapScreens.Add(new SnapScreen(null));

            SelectedSnapScreen = SnapScreens.First();

            Layouts = new ObservableCollection<Layout>();
            Layouts.Add(new Layout { Name = "Layout 1" });
            Layouts.Add(new Layout { Name = "Layout 2" });

            SelectedLayout = Layouts.First();
        }
    }
}