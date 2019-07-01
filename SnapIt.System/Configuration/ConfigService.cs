using System.IO;
using Newtonsoft.Json;

namespace SnapIt.Configuration
{
    public class ConfigService : IConfigService
    {
        public void Save<T>(T config)
        {
            var configPath = GetConfigPath<T>();

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            File.WriteAllText(configPath, json);
        }

        public T Load<T>()
        {
            var configPath = GetConfigPath<T>();

            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, string.Empty);
            }

            var json = File.ReadAllText(configPath);

            return JsonConvert.DeserializeObject<T>(json);
        }

        private string GetConfigPath<T>()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), $"{nameof(T)}.json");
        }
    }
}