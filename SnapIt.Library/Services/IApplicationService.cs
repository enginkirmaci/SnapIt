using System.Collections.Generic;
using System.Threading.Tasks;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface IApplicationService
    {
        void Initialize();

        void Clear();

        List<ApplicationItem> ListInstalledApplications();

        Task<ActiveWindow> StartApplication(ApplicationItem application, Rectangle rectangle);
    }
}