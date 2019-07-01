namespace SnapIt.Configuration
{
    public class ConfigItem<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }
    }
}