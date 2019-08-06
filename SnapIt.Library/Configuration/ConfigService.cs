using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Configuration
{
    public class ConfigService : IConfigService
    {
        private JsonSerializerSettings jsonSerializerSettings;
        private readonly string rootFolder;

        public ConfigService()
        {
            rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

            Directory.CreateDirectory(rootFolder);

            InitializeLayouts();

            jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }

        private void InitializeLayouts()
        {
            var layouts = new string[]{
                "0e217130-d44e-4057-bfe9-daf6f6e341d2",
                "335af962-69ad-4b2e-a2ef-77cd0f1fc329",
                "377f159c-72bd-4876-bdbe-85745e10f3cc",
                "792fc82f-65dd-4795-a589-b6b39a21d9ef",
                "00904f1f-4e41-4dc6-8677-6e5a2b231935",
                "e5e6b3d7-cb2c-4e90-92b7-e7f5dbd35f95",
                "eba07116-7ba0-40ec-80f0-fa9542afc640",
                "effbbdbb-1fe0-4639-8f8b-3f49156d5b2c",
                "f216a979-b3f3-427a-9c31-7977eaf91b19",
                "f640fa94-b4ea-4755-9b6f-68bf49b85c0c",
                "f332538f-5f83-4c4f-8472-1155f1aef340"
            };

            var layoutsFolder = Path.Combine(rootFolder, "Layouts");
            Directory.CreateDirectory(layoutsFolder);

            foreach (var layout in layouts)
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"SnapIt.Library.Layouts.{layout}.json"))
                {
                    using (TextReader tr = new StreamReader(stream))
                    {
                        string fileContents = tr.ReadToEnd();

                        File.WriteAllText(Path.Combine(rootFolder, $"Layouts\\{layout}.json"), fileContents);
                    }
                }
            }
        }

        public void Save<T>(T config)
        {
            var configPath = GetConfigPath<T>();

            var json = JsonConvert.SerializeObject(config, jsonSerializerSettings);

            File.WriteAllText(configPath, json);
        }

        public T Load<T>() where T : new()
        {
            var configPath = GetConfigPath<T>();

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(new T(), jsonSerializerSettings));
            }

            var json = File.ReadAllText(configPath);

            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
        }

        public void SaveLayout(Layout layout)
        {
            var layoutPath = GetLayoutPath(layout);

            var json = JsonConvert.SerializeObject(layout, jsonSerializerSettings);

            File.WriteAllText(layoutPath, json);
        }

        public void ExportLayout(Layout layout, string layoutPath)
        {
            var json = JsonConvert.SerializeObject(layout, jsonSerializerSettings);

            File.WriteAllText(layoutPath, json);
        }

        public Layout ImportLayout(string layoutPath)
        {
            var json = File.ReadAllText(layoutPath);

            var layout = JsonConvert.DeserializeObject<Layout>(json, jsonSerializerSettings);

            SaveLayout(layout);

            return layout;
        }

        public IList<Layout> GetLayouts()
        {
            var folderPath = Path.Combine(rootFolder, "Layouts");
            var files = Directory.GetFiles(folderPath, "*.json");
            var layouts = new List<Layout>();

            foreach (var file in files)
            {
                var layout = JsonConvert.DeserializeObject<Layout>(File.ReadAllText(file));
                layout.IsSaved = true;
                layouts.Add(layout);
            }

            return layouts.OrderBy(i => i.Name).ToList();
        }

        private string GetConfigPath<T>()
        {
            return Path.Combine(rootFolder, $"{typeof(T).Name}.json");
        }

        private string GetLayoutPath(Layout layout)
        {
            return Path.Combine(rootFolder, $"Layouts\\{layout.Guid}.json");
        }
    }
}