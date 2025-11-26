namespace SnapIt.Services.Database.Entities;

/// <summary>
/// Database entity for Layout
/// </summary>
public class LayoutEntity
{
    public int Id { get; set; }
    public string Guid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "2.0";
    public string JsonData { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}
