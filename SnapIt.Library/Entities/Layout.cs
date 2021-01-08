using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using SnapIt.Library.Controls;

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
        public List<LayoutArea> LayoutAreas { get; set; }

        //TODO remove after refactoring
        public LayoutAreaOld LayoutArea { get => layoutArea; set => SetProperty(ref layoutArea, value); }

        public Layout()
        {
            LayoutLines = new List<LayoutLine>();
            LayoutAreas = new List<LayoutArea>();
        }

        public void GenerateLayoutArea(SnapArea snapArea)
        {
            //TODO here
            //LayoutArea = new LayoutArea();
            //snapArea.GetLayoutAreas(LayoutArea);
        }
    }
}