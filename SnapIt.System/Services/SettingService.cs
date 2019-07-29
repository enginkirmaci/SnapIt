using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SnapIt.Configuration;
using SnapIt.Controls;
using SnapIt.Entities;

namespace SnapIt.Services
{
	public class SettingService : ISettingService
	{
		//private readonly ISnapService snapService;
		private readonly IConfigService configService;

		public Config Config { get; private set; }
		public IList<Layout> Layouts { get; private set; }
		public IList<SnapScreen> SnapScreens { get; private set; }

		public SettingService(
			//ISnapService snapService,
			IConfigService configService)
		{
			//this.snapService = snapService;
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

			//snapService.Release();
			//snapService.Initialize();
		}

		public void SaveLayout(Layout layout)
		{
			layout.IsSaved = true;
			configService.SaveLayout(layout);
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

				snapScreens.Add(snapScreen);
			}

			return snapScreens;
		}
	}
}