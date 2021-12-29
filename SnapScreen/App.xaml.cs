using System.Windows;
using DryIoc;
using Prism.Ioc;
using SnapScreen.Views;

namespace SnapScreen
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            var applicationWindow = Container.Resolve<MainWindow>();

            return applicationWindow;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}