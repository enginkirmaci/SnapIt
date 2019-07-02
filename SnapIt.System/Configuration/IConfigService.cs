namespace SnapIt.Configuration
{
    public interface IConfigService
    {
        void Save<T>(T config);

        T Load<T>() where T : new();
    }
}