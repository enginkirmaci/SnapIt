using System.Collections.Generic;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Configuration
{
	public interface IConfigService
	{
		void Save<T>(T config);

		T Load<T>() where T : new();

		void SaveLayout(Layout layout);

		IList<Layout> GetLayouts();
	}
}