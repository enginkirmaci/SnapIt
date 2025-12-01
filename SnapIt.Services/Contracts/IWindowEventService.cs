using SnapIt.Common.Contracts;

namespace SnapIt.Services.Contracts;

public interface IWindowEventService : IInitialize
{
    void StartMonitoring();
    void StopMonitoring();
    bool IsMonitoring { get; }
}
