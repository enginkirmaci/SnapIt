using System.Threading.Tasks;
using System.Windows;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.Services
{
    public interface IStoreLicenseService
    {
        event OfflineLicensesChangedEvent OfflineLicensesChanged;

        Task<PurchaseStatus> RequestPurchaseAsync();

        Task<LicenseStatus> CheckStatusAsync();

        void Init(Window window);
    }

    public delegate void OfflineLicensesChangedEvent();
}