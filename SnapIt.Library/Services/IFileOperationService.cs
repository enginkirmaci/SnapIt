using SnapIt.Library.Entities;
using System.Collections.Generic;

namespace SnapIt.Library.Services
{
    public interface IFileOperationService
    {
        void Save<T>(T config);

        T Load<T>() where T : new();

        void SaveLayout(Layout layout);

        void ExportLayout(Layout layout, string layoutPath);

        void DeleteLayout(Layout layout);

        Layout ImportLayout(string layoutPath);

        IList<Layout> GetLayouts();
    }
}