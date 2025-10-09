using SnapIt.Common.Contracts;

namespace SnapIt.Application.Contracts;

public interface IScreenManager : IInitialize
{
    void SetSnapManager(ISnapManager snapManager);
}