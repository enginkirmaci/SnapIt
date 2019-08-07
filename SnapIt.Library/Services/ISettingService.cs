using System.Collections.Generic;
using System.Threading.Tasks;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
	public interface ISettingService
	{
		Config Config { get; }
		IList<Layout> Layouts { get; }
		IList<SnapScreen> SnapScreens { get; }

		void Save();

		void SaveLayout(Layout layout);

		void ExportLayout(Layout layout, string layoutPath);

		Layout ImportLayout(string layoutPath);

		void LinkScreenLayout(SnapScreen snapScreen, Layout layout);

		Task<bool> GetStartupTaskStatusAsync();

		Task SetStartupTaskStatusAsync(bool isActive);
	}
}