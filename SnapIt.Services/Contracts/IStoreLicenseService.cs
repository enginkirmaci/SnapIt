using System.Windows;
using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;

namespace SnapIt.Services.Contracts;

public interface IStoreLicenseService : IInitialize
{
    event OfflineLicensesChangedEvent OfflineLicensesChanged;

    Task<PurchaseStatus> RequestPurchaseAsync();

    Task<LicenseStatus> CheckStatusAsync();

    void Init(Window window);
}

public delegate void OfflineLicensesChangedEvent();