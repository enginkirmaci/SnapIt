using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Controls;

namespace SnapIt.UI.ViewModels
{
    public class DesignWindowViewModel : BindableBase
    {
        public SnapArea MainSnapArea { get; set; }
        public DelegateCommand<object> LoadedCommand { get; }

        public DesignWindowViewModel()
        {
            LoadedCommand = new DelegateCommand<object>((mainSnapArea) =>
            {
                MainSnapArea = mainSnapArea as SnapArea;
                MainSnapArea.SetDesignMode(null);
            });
        }
    }
}