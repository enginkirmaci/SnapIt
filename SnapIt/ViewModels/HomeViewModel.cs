using System.Windows;
using Prism.Commands;
using Prism.Mvvm;

namespace SnapIt.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        public DelegateCommand<string> NavigateCommand { get; private set; }

        public HomeViewModel()
        {
            NavigateCommand = new DelegateCommand<string>((navigatePath) =>
            {
                var mainWindow = Application.Current.MainWindow;

                ((MainWindowViewModel)mainWindow.DataContext).NavigateView(navigatePath);
            });
        }
    }
}