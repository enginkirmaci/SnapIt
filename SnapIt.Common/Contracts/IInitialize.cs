namespace SnapIt.Common.Contracts;

public interface IInitialize
{
    public bool IsInitialized { get; }

    Task InitializeAsync();
}