namespace SnapIt.Services.Database.Entities;

/// <summary>
/// Database entity for StandaloneLicense
/// </summary>
public class StandaloneLicenseEntity
{
    public int Id { get; set; }
    public string JsonData { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
}
