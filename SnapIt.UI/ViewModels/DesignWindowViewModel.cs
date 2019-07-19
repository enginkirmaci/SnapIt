using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Controls;
using SnapIt.Entities;

namespace SnapIt.UI.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        public SnapScreen SnapScreen { get; set; }
        public Layout Layout { get; set; }
        public SnapArea MainSnapArea { get; set; }
        public DelegateCommand<object> LoadedCommand { get; }
        public DelegateCommand SaveLayoutCommand { get; private set; }

        public DesignWindowViewModel()
        {
            LoadedCommand = new DelegateCommand<object>((mainSnapArea) =>
            {
                MainSnapArea = mainSnapArea as SnapArea;
                //MainSnapArea.SetDesignMode(null); //remove this
                MainSnapArea.ApplyLayout(Layout.LayoutArea);
            });

            SaveLayoutCommand = new DelegateCommand(() =>
            {
                Layout.GenerateLayoutArea(MainSnapArea);
            });
        }
    }
}