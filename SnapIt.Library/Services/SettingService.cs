using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnapIt.Library.Configuration;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using Windows.ApplicationModel;

namespace SnapIt.Library.Services
{
	public class SettingService : ISettingService
	{
		private readonly IConfigService configService;

		public Config Config { get; private set; }
		public IList<Layout> Layouts { get; private set; }
		public IList<SnapScreen> SnapScreens { get; private set; }

		public SettingService(
			IConfigService configService)
		{
			this.configService = configService;

			Config = configService.Load<Config>();
			Layouts = configService.GetLayouts();
			SnapScreens = GetSnapScreens();
		}

		public void Save()
		{
			configService.Save(Config);

			foreach (var layout in Layouts.Where(i => !i.IsSaved))
			{
				SaveLayout(layout);
			}
		}

		public void SaveLayout(Layout layout)
		{
			layout.IsSaved = true;
			configService.SaveLayout(layout);
		}

		public void ExportLayout(Layout layout, string layoutPath)
		{
			configService.ExportLayout(layout, layoutPath);
		}

		public Layout ImportLayout(string layoutPath)
		{
			return configService.ImportLayout(layoutPath);
		}

		public void LinkScreenLayout(SnapScreen snapScreen, Layout layout)
		{
			SnapScreens.First(screen => screen.Base.DeviceName == snapScreen.Base.DeviceName).Layout = layout;

			if (Config.ScreensLayouts.ContainsKey(snapScreen.Base.DeviceName))
			{
				Config.ScreensLayouts[snapScreen.Base.DeviceName] = layout.Guid.ToString();
			}
			else
			{
				Config.ScreensLayouts.Add(snapScreen.Base.DeviceName, layout.Guid.ToString());
			}
		}

		private IList<SnapScreen> GetSnapScreens()
		{
			var snapScreens = new List<SnapScreen>();

			foreach (var screen in Screen.AllScreens)
			{
				var snapScreen = new SnapScreen(screen);
				var layoutGuid = Config.ScreensLayouts.ContainsKey(snapScreen.Base.DeviceName)
					? Config.ScreensLayouts[snapScreen.Base.DeviceName] : string.Empty;

				if (!string.IsNullOrWhiteSpace(layoutGuid))
				{
					snapScreen.Layout = Layouts.FirstOrDefault(layout => layout.Guid.ToString() == layoutGuid);
				}
				else
				{
					snapScreen.Layout = Layouts.FirstOrDefault();
				}

				snapScreens.Add(snapScreen);
			}

			return snapScreens;
		}

		public async Task<bool> GetStartupTaskStatusAsync()
		{
			try
			{
				var startupTask = await StartupTask.GetAsync("SnapItStartupTask"); // Pass the task ID you specified in the appxmanifest file
				switch (startupTask.State)
				{
					case StartupTaskState.Disabled:
					case StartupTaskState.DisabledByUser:
					case StartupTaskState.DisabledByPolicy:
						return false;
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task SetStartupTaskStatusAsync(bool isActive)
		{
			try
			{
				var startupTask = await StartupTask.GetAsync("SnapItStartupTask");
				if (isActive)
				{
					await startupTask.RequestEnableAsync();
				}
				else
				{
					startupTask.Disable();
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}