using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;

namespace SnapIt.Library.Entities
{
    public class Layout : Bindable
    {
        private string name;
        private LayoutAreaOld layoutArea;

        public string Version = "2.0";
        public Guid Guid { get; set; }
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

        //TODO remove after refactoring
        //public LayoutAreaOld LayoutArea { get => layoutArea; set => SetProperty(ref layoutArea, value); }

        public Layout()
        {
            LayoutLines = new List<LayoutLine>();
        }
    }
}