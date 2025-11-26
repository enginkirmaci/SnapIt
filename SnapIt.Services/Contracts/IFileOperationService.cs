using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;

namespace SnapIt.Services.Contracts;

public interface IFileOperationService : IInitialize
{
    Task SaveAsync<T>(T config);

    Task<T> LoadAsync<T>() where T : new();

    void SaveLayout(Layout layout);

    void ExportLayout(Layout layout, string layoutPath);

    void DeleteLayout(Layout layout);

    Layout ImportLayout(string layoutPath);

    IList<Layout> GetLayouts();
}