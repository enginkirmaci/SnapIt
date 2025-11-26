namespace SnapIt.Services.Database.Entities;

/// <summary>
/// Database entity for ApplicationGroupSettings
/// </summary>
public class ApplicationGroupSettingsEntity
{
    public int Id { get; set; }
    public string Version { get; set; } = "1.0";
    public string JsonData { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}
