using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using SnapIt.Library.Entities;
using SnapIt.ViewModels.DesignModels;

namespace SnapIt.ViewModels
{
    public class LayoutDesignView
    {
        public ObservableCollection<SnapScreen> SnapScreens { get; set; }
        public SnapScreen SelectedSnapScreen { get; set; }
        public ObservableCollection<Layout> Layouts { get; set; }
        public Layout SelectedLayout { get; set; }
        public DelegateCommand DesignLayoutCommand { get; private set; }
        public DelegateCommand ExportLayoutCommand { get; private set; }

        public LayoutDesignView()
        {
            SnapScreens = new ObservableCollection<SnapScreen>();
            SnapScreens.Add(new SnapScreen() { DeviceNumber = "1", Primary = "Primary", Resolution = "1920 x 1080" });
            SnapScreens.Add(new SnapScreen() { DeviceNumber = "2", Primary = null, Resolution = "3440 x 1440" });

            Layouts = new ObservableCollection<Layout>();
            Layouts.Add(new Layout { Name = "Layout 1" });
            Layouts.Add(new Layout { Name = "Layout 2" });
            Layouts.Add(new Layout { Name = "3 Part Horizontal Reverse" });
            Layouts.Add(new Layout { Name = "Layout 4" });

            SelectedLayout = Layouts.First();
            SelectedSnapScreen = SnapScreens.First();
            SnapScreens[0].Layout = SelectedLayout;
            SnapScreens[1].Layout = SelectedLayout;
        }
    }
}

namespace SnapIt.ViewModels.DesignModels
{
    public class SnapScreen
    {
        public string Primary { get; set; }
        public string DeviceNumber { get; set; }
        public string Resolution { get; set; }
        public Layout Layout { get; set; }
    }
}