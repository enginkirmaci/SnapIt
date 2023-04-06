using System.Collections.Generic;

namespace SnapIt.Library.Entities
{
    public class ApplicationGroupSettings
    {
        public string Version = "1.0";

        public Dictionary<string, List<ApplicationGroup>> ScreensApplicationGroups { get; set; }
    }
}