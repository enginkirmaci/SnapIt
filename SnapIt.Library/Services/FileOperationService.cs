﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public class FileOperationService : IFileOperationService
    {
        private readonly string rootFolder;

        private readonly JsonSerializerSettings defaultJsonSerializerSettings;
        private readonly JsonSerializerSettings layoutJsonSerializerSettings;

        public FileOperationService()
        {
            rootFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

            Directory.CreateDirectory(rootFolder);

            InitializeLayouts();

            defaultJsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            layoutJsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }

        private void InitializeLayouts()
        {
            var layoutsFolder = Path.Combine(rootFolder, "Layouts");

            if (!Directory.Exists(layoutsFolder))
            {
                Directory.CreateDirectory(layoutsFolder);

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
        }

        public void Save<T>(T config)
        {
            var configPath = GetConfigPath<T>();

            var json = JsonConvert.SerializeObject(config, defaultJsonSerializerSettings);

            File.WriteAllText(configPath, json);
        }

        public T Load<T>() where T : new()
        {
            var configPath = GetConfigPath<T>();

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(new T(), defaultJsonSerializerSettings));
            }

            var json = File.ReadAllText(configPath);

            return JsonConvert.DeserializeObject<T>(json, defaultJsonSerializerSettings);
        }

        public void SaveLayout(Layout layout)
        {
            var layoutPath = GetLayoutPath(layout);

            var json = JsonConvert.SerializeObject(layout, layoutJsonSerializerSettings);

            File.WriteAllText(layoutPath, json);
        }

        public void ExportLayout(Layout layout, string layoutPath)
        {
            var json = JsonConvert.SerializeObject(layout, layoutJsonSerializerSettings);

            File.WriteAllText(layoutPath, json);
        }

        public void DeleteLayout(Layout layout)
        {
            var layoutPath = GetLayoutPath(layout);

            File.Delete(layoutPath);
        }

        public Layout ImportLayout(string layoutPath)
        {
            var json = File.ReadAllText(layoutPath);

            var layout = JsonConvert.DeserializeObject<Layout>(json, layoutJsonSerializerSettings);

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