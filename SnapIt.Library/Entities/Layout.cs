using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SnapIt.Library.Entities
{
    public class Layout : Bindable
    {
        private string name;

        public string Version = "2.0";
        public Guid Guid { get; set; }

        [JsonIgnore]
        public bool IsNew { get; set; }

        [JsonIgnore]
        public LayoutStatus Status { get; set; }

        [JsonIgnore]
        public SnapAreaTheme Theme { get; set; }

        public string Name
        {
            get => name;
            set
            {
                Status = LayoutStatus.NotSaved;
                SetProperty(ref name, value);
            }
        }

        public Size Size { get; set; }
        public List<LayoutLine> LayoutLines { get; set; }
        public List<LayoutOverlay> LayoutOverlays { get; set; }

        public Layout()
        {
            LayoutLines = new List<LayoutLine>();
            LayoutOverlays = new List<LayoutOverlay>();
        }
    }
}