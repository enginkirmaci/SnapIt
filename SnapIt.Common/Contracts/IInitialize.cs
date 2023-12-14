namespace SnapIt.Common.Contracts;

public interface IInitialize : IDisposable
{
    public bool IsInitialized { get; }

    Task InitializeAsync();
}