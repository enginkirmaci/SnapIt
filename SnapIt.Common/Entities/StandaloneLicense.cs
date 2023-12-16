namespace SnapIt.Common.Entities;

public class StandaloneLicense
{
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsValid { get; set; }

    public StandaloneLicense()
    {
        CreatedDate = DateTime.Now;
    }
}