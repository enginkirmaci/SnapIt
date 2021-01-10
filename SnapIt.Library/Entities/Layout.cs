using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;

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
        public bool IsSaved { get; set; }

        [JsonIgnore]
        public SnapAreaTheme Theme { get; set; }

        public string Name
        {
            get => name;
            set
            {
                IsSaved = false;
                SetProperty(ref name, value);
            }
        }

        public Size Size { get; set; }
        public List<LayoutLine> LayoutLines { get; set; }

        public Layout()
        {
            LayoutLines = new List<LayoutLine>();
        }
    }
}