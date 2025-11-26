using System.Collections.Concurrent;
using System.Reflection;
using SnapIt.Common.Entities;
using SnapIt.Common.Helpers;
using SnapIt.Layouts;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class FileOperationService : IFileOperationService
{
    private const string LayoutFolder = "Layoutsv20";

    private static readonly string[] PredefinedLayouts =
    {
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

    private readonly string _rootFolder = Constants.RootFolder;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();
    private volatile bool _isInitialized;

    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                return;
            }

            Directory.CreateDirectory(_rootFolder);
            InitializeLayouts();
            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public void Dispose()
    {
        _isInitialized = false;
        _initLock?.Dispose();

        foreach (var lockObj in _fileLocks.Values)
        {
            lockObj?.Dispose();
        }
        _fileLocks.Clear();
    }

    private void InitializeLayouts()
    {
        var layoutsFolder = Path.Combine(_rootFolder, LayoutFolder);

        if (Directory.Exists(layoutsFolder))
        {
            return;
        }

        Directory.CreateDirectory(layoutsFolder);

        var assembly = Assembly.GetAssembly(typeof(PredefinedLayout));

        foreach (var layoutId in PredefinedLayouts)
        {
            var resourceName = $"SnapIt.Layouts.{layoutId}.json";
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            var fileContents = reader.ReadToEnd();
            var filePath = Path.Combine(layoutsFolder, $"{layoutId}.json");

            File.WriteAllText(filePath, fileContents);
        }
    }

    public async Task SaveAsync<T>(T config)
    {
        var configPath = GetConfigPath<T>();
        var fileLock = GetFileLock(configPath);

        await fileLock.WaitAsync();
        try
        {
            var json = Json.Serialize(config);
            await File.WriteAllTextAsync(configPath, json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public async Task<T> LoadAsync<T>() where T : new()
    {
        var configPath = GetConfigPath<T>();
        var fileLock = GetFileLock(configPath);

        await fileLock.WaitAsync();
        try
        {
            if (!File.Exists(configPath))
            {
                var defaultJson = Json.Serialize(new T());
                await File.WriteAllTextAsync(configPath, defaultJson);
                return new T();
            }

            var json = await File.ReadAllTextAsync(configPath);
            return Json.Deserialize<T>(json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public void SaveLayout(Layout layout)
    {
        var layoutPath = GetLayoutPath(layout);
        var fileLock = GetFileLock(layoutPath);

        fileLock.Wait();
        try
        {
            var json = Json.Serialize(layout);
            File.WriteAllText(layoutPath, json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public void ExportLayout(Layout layout, string layoutPath)
    {
        var fileLock = GetFileLock(layoutPath);

        fileLock.Wait();
        try
        {
            var json = Json.Serialize(layout);
            File.WriteAllText(layoutPath, json);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public void DeleteLayout(Layout layout)
    {
        var layoutPath = GetLayoutPath(layout);
        var fileLock = GetFileLock(layoutPath);

        fileLock.Wait();
        try
        {
            if (File.Exists(layoutPath))
            {
                File.Delete(layoutPath);
            }
        }
        finally
        {
            fileLock.Release();
            _fileLocks.TryRemove(layoutPath, out _);
        }
    }

    public Layout ImportLayout(string layoutPath)
    {
        var fileLock = GetFileLock(layoutPath);

        fileLock.Wait();
        try
        {
            var json = File.ReadAllText(layoutPath);
            var layout = Json.Deserialize<Layout>(json);
            SaveLayout(layout);
            return layout;
        }
        finally
        {
            fileLock.Release();
        }
    }

    public IList<Layout> GetLayouts()
    {
        var folderPath = Path.Combine(_rootFolder, LayoutFolder);

        if (!Directory.Exists(folderPath))
        {
            return new List<Layout>();
        }

        var files = Directory.GetFiles(folderPath, "*.json");

        var layouts = files
            .AsParallel()
            .Select(file =>
            {
                try
                {
                    var fileLock = GetFileLock(file);
                    fileLock.Wait();
                    try
                    {
                        var layout = Json.Deserialize<Layout>(File.ReadAllText(file));
                        layout.Status = LayoutStatus.Saved;
                        return layout;
                    }
                    finally
                    {
                        fileLock.Release();
                    }
                }
                catch
                {
                    return null;
                }
            })
            .Where(layout => layout != null)
            .OrderBy(layout => layout?.Name)
            .ToList();

        return layouts;
    }

    private SemaphoreSlim GetFileLock(string filePath)
    {
        return _fileLocks.GetOrAdd(filePath, _ => new SemaphoreSlim(1, 1));
    }

    private string GetConfigPath<T>()
    {
        return Path.Combine(_rootFolder, $"{typeof(T).Name}.json");
    }

    private string GetLayoutPath(Layout layout)
    {
        return Path.Combine(_rootFolder, LayoutFolder, $"{layout.Guid}.json");
    }
}