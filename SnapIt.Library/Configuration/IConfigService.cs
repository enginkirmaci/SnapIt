using System.Collections.Generic;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Configuration
{
    public interface IConfigService
    {
        void Save<T>(T config);

        T Load<T>() where T : new();

        void SaveLayout(Layout layout);

        void ExportLayout(Layout layout, string layoutPath);

        Layout ImportLayout(string layoutPath);

        IList<Layout> GetLayouts();
    }
}