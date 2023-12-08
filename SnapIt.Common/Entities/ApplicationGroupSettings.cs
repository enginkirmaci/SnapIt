using System.Collections.Generic;

namespace SnapIt.Common.Entities;

public class ApplicationGroupSettings
{
    public string Version = "1.0";

    public Dictionary<string, List<ApplicationGroup>> ScreensApplicationGroups { get; set; }
}