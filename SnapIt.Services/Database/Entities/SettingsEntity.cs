namespace SnapIt.Services.Database.Entities;

/// <summary>
/// Database entity for Settings
/// </summary>
public class SettingsEntity
{
    public int Id { get; set; }
    public string Version { get; set; } = "2.0";
    public string JsonData { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}
