using System;
using Newtonsoft.Json;

namespace SnapIt.GridSplitter.Entities
{
    public class Layout : Bindable
    {
        private string name;
        private LayoutArea layoutArea;

        public string Version = "1.0";
        public Guid Guid { get; set; }
        [JsonIgnore]
        public bool IsSaved { get; set; }

        public string Name
        {
            get => name;
            set
            {
                IsSaved = false;
                SetProperty(ref name, value);
            }
        }

        public LayoutArea LayoutArea { get => layoutArea; set => SetProperty(ref layoutArea, value); }
    }
}