using System.IO;
using Newtonsoft.Json;

namespace SnapIt.Configuration
{
    public class ConfigService : IConfigService
    {
        private JsonSerializerSettings jsonSerializerSettings;

        public ConfigService()
        {
            jsonSerializerSettings = new JsonSerializerSettings
            {
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

        private string GetConfigPath<T>()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $"{typeof(T).Name}.json");
        }
    }
}