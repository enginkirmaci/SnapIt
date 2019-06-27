using System.Windows;
using Prism.Ioc;
using SnapIt.UI.Views;

namespace SnapIt.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.Register<ICopyManager, CopyManager>();
            //containerRegistry.Register<ICommandManager, CommandManager>();
            //containerRegistry.Register<IAuthenticationInitializer, AuthenticationInitializer>();

            //containerRegistry.RegisterSingleton<INotifierService, NotifierService>();
        }
    }
}