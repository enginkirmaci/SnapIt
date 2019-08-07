using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using DryIoc;
using Prism.Ioc;
using SnapIt.Library.Configuration;
using SnapIt.Library.Services;
using SnapIt.Views;

namespace SnapIt
{
	public partial class App
	{
		private static readonly Mutex mutex = new Mutex(true, "{FE4F369C-450C-4FA5-ACCA-3D261A3A7969}");

		protected override void OnStartup(StartupEventArgs e)
		{
			if (e.Args.Length > 0 && e.Args[0].Contains("runas"))
			{
				if (mutex.WaitOne(TimeSpan.Zero, true))
				{
					mutex.ReleaseMutex();
				}
				else
				{
					MessageBox.Show("only one instance at a time");

					Shutdown();
					return;
				}
			}
			else
			{
				var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

				ProcessStartInfo info = new ProcessStartInfo
				{
					Verb = "runas", // we'll run our EXE as admin
					Arguments = "-runas",
					UseShellExecute = true,
					FileName = localAppDataPath + @"\microsoft\windowsapps\SnapIt.exe" // path to the appExecutionAlias
				};
				Process.Start(info); // launch new elevated instance
				Shutdown(); // exit current instance
				return;
			}
			base.OnStartup(e);
		}

		protected override Window CreateShell()
		{
			var notifyIconService = Container.Resolve<INotifyIconService>();

			notifyIconService.Initialize();

			var applicationWindow = Container.Resolve<MainWindow>();

			notifyIconService.SetApplicationWindow(applicationWindow);

			return applicationWindow;
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterSingleton<INotifyIconService, NotifyIconService>();
			containerRegistry.RegisterSingleton<IConfigService, ConfigService>();
			containerRegistry.RegisterSingleton<ISnapService, SnapService>();
			containerRegistry.RegisterSingleton<ISettingService, SettingService>();
			containerRegistry.Register<IWindowService, WindowService>();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			if (Container != null)
			{
				Container.Resolve<INotifyIconService>().Release();
				Container.Resolve<ISnapService>().Release();
			}
		}
	}
}