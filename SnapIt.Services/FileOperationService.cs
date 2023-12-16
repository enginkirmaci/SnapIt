using System.Reflection;
using SnapIt.Common.Entities;
using SnapIt.Common.Helpers;
using SnapIt.Layouts;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class FileOperationService : IFileOperationService
{
    private const string LayoutFolder = "Layoutsv20";
    private readonly string rootFolder = Constants.RootFolder;

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        Directory.CreateDirectory(rootFolder);

        InitializeLayouts();

        IsInitialized = true;
    }

    public void Dispose()
    {
        IsInitialized = false;
    }

    private void InitializeLayouts()
    {
        var layoutsFolder = Path.Combine(rootFolder, LayoutFolder);

        if (!Directory.Exists(layoutsFolder))
        {
            Directory.CreateDirectory(layoutsFolder);

            var layouts = new string[]{
                "097d074b-0eed-4878-a035-b2eb76a324ce",
                "1ff17736-c6cf-49be-b63e-a5affefd31d9",
                "338106eb-1ae5-4185-bc00-f46339e1888d",
                "61c00b23-5cdf-4c27-9a6d-a73693f16d47",
                "6a86804e-c948-47ea-a1c3-1387736a8a80",
                "6a91f112-af26-4505-86cb-a1983e4f4e14",
                "97565260-e874-41dd-849e-0351e5dcbc6e",
                "a4e1eb3d-376d-473c-afac-cb253cd8ee8e",
                "b94079de-54a2-49d0-b31c-878a8f63ba75",
                "edcedeaf-acb2-483a-86ae-ccc7c021f9c9",
                "f1cb61d1-d38a-4e80-adab-e4be62d057f3"
            };

            foreach (var layout in layouts)
            {
                using (var stream = Assembly.GetAssembly(typeof(PredefinedLayout)).GetManifestResourceStream($"SnapIt.Layouts.{layout}.json"))
                {
                    using (TextReader tr = new StreamReader(stream))
                    {
                        string fileContents = tr.ReadToEnd();

                        File.WriteAllText(Path.Combine(rootFolder, $"{LayoutFolder}\\{layout}.json"), fileContents);
                    }
                }
            }
        }
    }

    public async Task Save<T>(T config)
    {
        var configPath = GetConfigPath<T>();

        var json = Json.Serialize(config);

        File.WriteAllText(configPath, json);
    }

    public async Task<T> Load<T>() where T : new()
    {
        var configPath = GetConfigPath<T>();

        if (!File.Exists(configPath))
        {
            await File.WriteAllTextAsync(configPath, Json.Serialize(new T()));
        }

        var json = await File.ReadAllTextAsync(configPath);

        return Json.Deserialize<T>(json);
    }

    public void SaveLayout(Layout layout)
    {
        var layoutPath = GetLayoutPath(layout);

        var json = Json.Serialize(layout);

        File.WriteAllText(layoutPath, json);
    }

    public void ExportLayout(Layout layout, string layoutPath)
    {
        var json = Json.Serialize(layout);

        File.WriteAllText(layoutPath, json);
    }

    public void DeleteLayout(Layout layout)
    {
        var layoutPath = GetLayoutPath(layout);

        File.Delete(layoutPath);
    }

    public Layout ImportLayout(string layoutPath)
    {
        var json = File.ReadAllText(layoutPath);

        var layout = Json.Deserialize<Layout>(json);

        SaveLayout(layout);

        return layout;
    }

    public IList<Layout> GetLayouts()
    {
        var folderPath = Path.Combine(rootFolder, LayoutFolder);
        var files = Directory.GetFiles(folderPath, "*.json");
        var layouts = new List<Layout>();

        foreach (var file in files)
        {
            var layout = Json.Deserialize<Layout>(File.ReadAllText(file));
            layout.Status = LayoutStatus.Saved;
            layouts.Add(layout);
        }

        return layouts.OrderBy(i => i.Name).ToList();
    }

    private string GetConfigPath<T>()
    {
        return Path.Combine(rootFolder, $"{typeof(T).Name}.json");
    }

    private string GetLayoutPath(Layout layout)
    {
        return Path.Combine(rootFolder, $"{LayoutFolder}\\{layout.Guid}.json");
    }
}