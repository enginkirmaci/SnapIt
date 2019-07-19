using System;
using System.IO;
using Newtonsoft.Json;
using SnapIt.Controls;

namespace SnapIt.Entities
{
    public class Layout : Bindable
    {
        private string name;

        public Guid Guid { get; set; }
        public string Name { get => name; set => SetProperty(ref name, value); }

        public LayoutArea LayoutArea { get; set; }

        public void Save()
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"Layouts\\{Name}_{Guid}.json");

            var json = JsonConvert.SerializeObject(this, settings);

            File.WriteAllText(filePath, json);
        }

        public void GenerateLayoutArea(SnapArea snapArea)
        {
            LayoutArea = new LayoutArea();
            snapArea.GetLayoutAreas(LayoutArea);
        }
    }
}