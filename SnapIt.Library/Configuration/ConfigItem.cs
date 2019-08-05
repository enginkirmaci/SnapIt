namespace SnapIt.Library.Configuration
{
	public class ConfigItem<T>
	{
		public ConfigItem(string key, T value)
		{
			Key = key;
			Value = value;
		}

		public string Key { get; set; }
		public T Value { get; set; }
	}
}