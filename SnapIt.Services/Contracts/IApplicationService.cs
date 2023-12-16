using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Services.Contracts;

public interface IApplicationService : IInitialize
{
    void Clear();

    List<ApplicationItem> ListInstalledApplications();

    Task<ActiveWindow> StartApplication(ApplicationItem application, Rectangle rectangle);
}