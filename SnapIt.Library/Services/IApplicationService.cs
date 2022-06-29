using System.Threading.Tasks;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface IApplicationService
    {
        void Initialize();

        void Clear();

        Task<ActiveWindow> StartApplication(ApplicationItem application, Rectangle rectangle);
    }
}