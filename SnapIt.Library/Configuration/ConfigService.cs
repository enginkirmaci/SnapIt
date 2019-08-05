using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Configuration
{
	public class ConfigService : IConfigService
	{
		private JsonSerializerSettings jsonSerializerSettings;

		public ConfigService()
		{
			jsonSerializerSettings = new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				Formatting = Formatting.Indented
			};
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

		public IList<Layout> GetLayouts()
		{
			var folderPath = Path.Combine(Application.StartupPath, "Layouts");
			var files = Directory.GetFiles(folderPath, "*.json");
			var layouts = new List<Layout>();

			foreach (var file in files)
			{
				var layout = JsonConvert.DeserializeObject<Layout>(File.ReadAllText(file));
				layout.IsSaved = true;
				layouts.Add(layout);
			}

			return layouts;
		}

		private string GetConfigPath<T>()
		{
			return Path.Combine(Application.StartupPath, $"{typeof(T).Name}.json");
		}

		private string GetLayoutPath(Layout layout)
		{
			return Path.Combine(Application.StartupPath, $"Layouts\\{layout.Guid}.json");
		}
	}
}