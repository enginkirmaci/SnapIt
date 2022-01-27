using System.Collections.Generic;

namespace SnapIt.Library.Entities
{
    public class ExcludedApplicationSettings
    {
        public string Version = "2.0";
        public List<ExcludedApplication> Applications { get; set; }
    }
}