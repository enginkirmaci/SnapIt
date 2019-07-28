using System.Collections.Generic;
using SnapIt.Entities;

namespace SnapIt.Configuration
{
	public interface IConfigService
	{
		void Save<T>(T config);

		T Load<T>() where T : new();

		void SaveLayout(Layout layout);

		IList<Layout> GetLayouts();
	}
}