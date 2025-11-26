using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SnapIt.Common.Entities;
using SnapIt.Common.Helpers;
using SnapIt.Layouts;
using SnapIt.Services.Contracts;
using SnapIt.Services.Database;
using SnapIt.Services.Database.Entities;

namespace SnapIt.Services;

/// <summary>
/// Database-based implementation of IFileOperationService using SQLite.
/// Migrates existing JSON files to SQLite on first initialization.
/// </summary>
public class DatabaseOperationService : IFileOperationService
{
    private const string LayoutFolder = "Layoutsv20";
    private const string DatabaseFileName = "SnapItSettings.db";

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
    private readonly string _databasePath;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _operationLocks = new();
    private volatile bool _isInitialized;

    public bool IsInitialized => _isInitialized;

    public DatabaseOperationService()
    {
        _databasePath = Path.Combine(_rootFolder, DatabaseFileName);
    }

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

            // Initialize database
            using (var context = new SettingsDbContext(_databasePath))
            {
                await context.Database.EnsureCreatedAsync();
            }

            // Migrate from JSON files if this is first run
            await MigrateFromJsonIfNeededAsync();

            // Ensure predefined layouts exist
            await InitializeLayoutsAsync();

            _isInitialized = true;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public void Dispose()
    {
        foreach (var lockObj in _operationLocks.Values)
        {
            try
            {
                lockObj?.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
        _operationLocks.Clear();

        try
        {
            _initLock?.Dispose();
        }
        catch
        {
            // Ignore disposal errors
        }

        _isInitialized = false;
    }

    public async Task SaveAsync<T>(T config)
    {
        var typeName = typeof(T).Name;
        var operationLock = GetOperationLock(typeName);

        await operationLock.WaitAsync();
        try
        {
            var jsonData = Json.Serialize(config);
            if (jsonData == null)
            {
                return;
            }

            using var context = new SettingsDbContext(_databasePath);

            if (typeName == nameof(Settings))
            {
                var entity = await context.Settings.FirstOrDefaultAsync();
                if (entity == null)
                {
                    entity = new SettingsEntity
                    {
                        Version = "2.0",
                        JsonData = jsonData,
                        LastModified = DateTime.UtcNow
                    };
                    context.Settings.Add(entity);
                }
                else
                {
                    entity.JsonData = jsonData;
                    entity.LastModified = DateTime.UtcNow;
                }
            }
            else if (typeName == nameof(ExcludedApplicationSettings))
            {
                var entity = await context.ExcludedApplicationSettings.FirstOrDefaultAsync();
                if (entity == null)
                {
                    entity = new ExcludedApplicationSettingsEntity
                    {
                        Version = "2.0",
                        JsonData = jsonData,
                        LastModified = DateTime.UtcNow
                    };
                    context.ExcludedApplicationSettings.Add(entity);
                }
                else
                {
                    entity.JsonData = jsonData;
                    entity.LastModified = DateTime.UtcNow;
                }
            }
            else if (typeName == nameof(ApplicationGroupSettings))
            {
                var entity = await context.ApplicationGroupSettings.FirstOrDefaultAsync();
                if (entity == null)
                {
                    entity = new ApplicationGroupSettingsEntity
                    {
                        Version = "1.0",
                        JsonData = jsonData,
                        LastModified = DateTime.UtcNow
                    };
                    context.ApplicationGroupSettings.Add(entity);
                }
                else
                {
                    entity.JsonData = jsonData;
                    entity.LastModified = DateTime.UtcNow;
                }
            }
            else if (typeName == nameof(StandaloneLicense))
            {
                var entity = await context.StandaloneLicenses.FirstOrDefaultAsync();
                if (entity == null)
                {
                    entity = new StandaloneLicenseEntity
                    {
                        JsonData = jsonData,
                        LastModified = DateTime.UtcNow
                    };
                    context.StandaloneLicenses.Add(entity);
                }
                else
                {
                    entity.JsonData = jsonData;
                    entity.LastModified = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync();
        }
        finally
        {
            operationLock.Release();
        }
    }

    public async Task<T> LoadAsync<T>() where T : new()
    {
        var typeName = typeof(T).Name;
        var operationLock = GetOperationLock(typeName);

        await operationLock.WaitAsync();
        try
        {
            using var context = new SettingsDbContext(_databasePath);

            string? jsonData = null;

            if (typeName == nameof(Settings))
            {
                var entity = await context.Settings.FirstOrDefaultAsync();
                jsonData = entity?.JsonData;
            }
            else if (typeName == nameof(ExcludedApplicationSettings))
            {
                var entity = await context.ExcludedApplicationSettings.FirstOrDefaultAsync();
                jsonData = entity?.JsonData;
            }
            else if (typeName == nameof(ApplicationGroupSettings))
            {
                var entity = await context.ApplicationGroupSettings.FirstOrDefaultAsync();
                jsonData = entity?.JsonData;
            }
            else if (typeName == nameof(StandaloneLicense))
            {
                var entity = await context.StandaloneLicenses.FirstOrDefaultAsync();
                jsonData = entity?.JsonData;
            }

            if (string.IsNullOrEmpty(jsonData))
            {
                var defaultValue = new T();
                _ = SaveAsync(defaultValue);
                return defaultValue;
            }

            return Json.Deserialize<T>(jsonData) ?? new T();
        }
        finally
        {
            operationLock.Release();
        }
    }

    public void SaveLayout(Layout layout)
    {
        var operationLock = GetOperationLock($"Layout_{layout.Guid}");

        operationLock.Wait();
        try
        {
            var jsonData = Json.Serialize(layout);
            if (jsonData == null)
            {
                return;
            }

            using var context = new SettingsDbContext(_databasePath);

            var entity = context.Layouts.FirstOrDefault(l => l.Guid == layout.Guid.ToString());
            if (entity == null)
            {
                entity = new LayoutEntity
                {
                    Guid = layout.Guid.ToString(),
                    Name = layout.Name,
                    Version = layout.Version,
                    JsonData = jsonData,
                    LastModified = DateTime.UtcNow
                };
                context.Layouts.Add(entity);
            }
            else
            {
                entity.Name = layout.Name;
                entity.JsonData = jsonData;
                entity.LastModified = DateTime.UtcNow;
            }

            context.SaveChanges();
        }
        catch (Exception)
        {
            // Log or handle layout save errors if needed
            throw;
        }
        finally
        {
            operationLock.Release();
        }
    }

    public void ExportLayout(Layout layout, string layoutPath)
    {
        var operationLock = GetOperationLock($"Export_{layoutPath}");

        operationLock.Wait();
        try
        {
            var json = Json.Serialize(layout);
            File.WriteAllText(layoutPath, json);
        }
        finally
        {
            operationLock.Release();
        }
    }

    public void DeleteLayout(Layout layout)
    {
        var operationLock = GetOperationLock($"Layout_{layout.Guid}");

        operationLock.Wait();
        try
        {
            using var context = new SettingsDbContext(_databasePath);

            var entity = context.Layouts.FirstOrDefault(l => l.Guid == layout.Guid.ToString());
            if (entity != null)
            {
                context.Layouts.Remove(entity);
                context.SaveChanges();
            }
        }
        catch (Exception)
        {
            // Log or handle layout deletion errors if needed
            throw;
        }
        finally
        {
            operationLock.Release();
            _operationLocks.TryRemove($"Layout_{layout.Guid}", out _);
        }
    }

    public Layout ImportLayout(string layoutPath)
    {
        var operationLock = GetOperationLock($"Import_{layoutPath}");

        operationLock.Wait();
        try
        {
            var json = File.ReadAllText(layoutPath);
            var layout = Json.Deserialize<Layout>(json);
            if (layout != null)
            {
                SaveLayout(layout);
            }
            return layout ?? new Layout();
        }
        finally
        {
            operationLock.Release();
        }
    }

    public IList<Layout> GetLayouts()
    {
        using var context = new SettingsDbContext(_databasePath);

        var layouts = context.Layouts
            .AsNoTracking()
            .AsEnumerable() // Switch to client-side evaluation for JSON deserialization
            .Select(entity =>
            {
                try
                {
                    var layout = Json.Deserialize<Layout>(entity.JsonData);
                    if (layout != null)
                    {
                        layout.Status = LayoutStatus.Saved;
                        return layout;
                    }
                    return null;
                }
                catch (Exception)
                {
                    // Skip invalid layout data
                    return null;
                }
            })
            .Where(layout => layout != null)
            .OrderBy(layout => layout?.Name)
            .Cast<Layout>()
            .ToList();

        return layouts;
    }

    private async Task InitializeLayoutsAsync()
    {
        using var context = new SettingsDbContext(_databasePath);

        // Check if layouts already exist in database
        var existingLayoutCount = await context.Layouts.CountAsync();
        if (existingLayoutCount > 0)
        {
            return;
        }

        // Load predefined layouts from embedded resources
        var assembly = Assembly.GetAssembly(typeof(PredefinedLayout));

        foreach (var layoutId in PredefinedLayouts)
        {
            try
            {
                var resourceName = $"SnapIt.Layouts.{layoutId}.json";
                using var stream = assembly?.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    continue;
                }

                using var reader = new StreamReader(stream);
                var fileContents = await reader.ReadToEndAsync();
                var layout = Json.Deserialize<Layout>(fileContents);

                if (layout != null)
                {
                    var entity = new LayoutEntity
                    {
                        Guid = layout.Guid.ToString(),
                        Name = layout.Name,
                        Version = layout.Version,
                        JsonData = fileContents,
                        LastModified = DateTime.UtcNow
                    };
                    context.Layouts.Add(entity);
                }
            }
            catch (Exception)
            {
                // Skip invalid predefined layout resource
                continue;
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task MigrateFromJsonIfNeededAsync()
    {
        using var context = new SettingsDbContext(_databasePath);

        // Check if migration is needed by checking if database is empty
        var hasSettings = await context.Settings.AnyAsync();
        if (hasSettings)
        {
            return; // Already migrated
        }

        // Migrate Settings
        var settingsPath = Path.Combine(_rootFolder, "Settings.json");
        if (File.Exists(settingsPath))
        {
            var jsonData = File.ReadAllText(settingsPath);
            var entity = new SettingsEntity
            {
                Version = "2.0",
                JsonData = jsonData,
                LastModified = DateTime.UtcNow
            };
            context.Settings.Add(entity);
        }

        // Migrate ExcludedApplicationSettings
        var excludedAppsPath = Path.Combine(_rootFolder, "ExcludedApplicationSettings.json");
        if (File.Exists(excludedAppsPath))
        {
            var jsonData = File.ReadAllText(excludedAppsPath);
            var entity = new ExcludedApplicationSettingsEntity
            {
                Version = "2.0",
                JsonData = jsonData,
                LastModified = DateTime.UtcNow
            };
            context.ExcludedApplicationSettings.Add(entity);
        }

        // Migrate ApplicationGroupSettings
        var appGroupPath = Path.Combine(_rootFolder, "ApplicationGroupSettings.json");
        if (File.Exists(appGroupPath))
        {
            var jsonData = File.ReadAllText(appGroupPath);
            var entity = new ApplicationGroupSettingsEntity
            {
                Version = "1.0",
                JsonData = jsonData,
                LastModified = DateTime.UtcNow
            };
            context.ApplicationGroupSettings.Add(entity);
        }

        // Migrate StandaloneLicense
        var licensePath = Path.Combine(_rootFolder, "StandaloneLicense.json");
        if (File.Exists(licensePath))
        {
            var jsonData = File.ReadAllText(licensePath);
            var entity = new StandaloneLicenseEntity
            {
                JsonData = jsonData,
                LastModified = DateTime.UtcNow
            };
            context.StandaloneLicenses.Add(entity);
        }

        // Migrate Layouts from files
        var layoutsFolder = Path.Combine(_rootFolder, LayoutFolder);
        if (Directory.Exists(layoutsFolder))
        {
            var layoutFiles = Directory.GetFiles(layoutsFolder, "*.json");
            foreach (var layoutFile in layoutFiles)
            {
                try
                {
                    var jsonData = File.ReadAllText(layoutFile);
                    var layout = Json.Deserialize<Layout>(jsonData);
                    if (layout != null)
                    {
                        var entity = new LayoutEntity
                        {
                            Guid = layout.Guid.ToString(),
                            Name = layout.Name,
                            Version = layout.Version,
                            JsonData = jsonData,
                            LastModified = DateTime.UtcNow
                        };
                        context.Layouts.Add(entity);
                    }
                }
                catch (Exception)
                {
                    // Skip invalid layout files during migration
                    continue;
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private SemaphoreSlim GetOperationLock(string key)
    {
        return _operationLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
    }
}