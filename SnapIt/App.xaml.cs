﻿using System;
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

			Container.Resolve<INotifyIconService>().Release();
			Container.Resolve<ISnapService>().Release();
		}
	}
}