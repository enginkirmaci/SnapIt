using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Services.Contracts;
using Windows.Services.Store;

namespace SnapIt.Services;

public class StoreLicenseService : IStoreLicenseService
{
    private StoreContext storeContext;

    public event OfflineLicensesChangedEvent OfflineLicensesChanged;

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        IsInitialized = true;
    }

    public void Init(Window window)
    {
        storeContext = StoreContext.GetDefault();
        storeContext.OfflineLicensesChanged += StoreOfflineLicensesChanged;

        //IInitializeWithWindow initWindow = (IInitializeWithWindow)(object)storeContext;
        //initWindow.Initialize(new WindowInteropHelper(window).Handle);

        WinRT.Interop.InitializeWithWindow.Initialize(storeContext, new WindowInteropHelper(window).Handle);
    }

    public async Task<PurchaseStatus> RequestPurchaseAsync()
    {
        var result = await storeContext.RequestPurchaseAsync(Constants.AppStoreId);

        if (result.ExtendedError != null)
        {
            return PurchaseStatus.Error;
        }

        switch (result.Status)
        {
            case StorePurchaseStatus.AlreadyPurchased:
                return PurchaseStatus.AlreadyPurchased;

            case StorePurchaseStatus.Succeeded:
                return PurchaseStatus.Succeeded;

            default:
                return PurchaseStatus.Error;
        }
    }

    private void StoreOfflineLicensesChanged(StoreContext sender, object args)
    {
        OfflineLicensesChanged?.Invoke();
    }

    public async Task<LicenseStatus> CheckStatusAsync()
    {
        var licenseStatus = LicenseStatus.InTrial;

        if (Dev.SkipLicense)
            return LicenseStatus.InTrial;

        var license = await storeContext.GetAppLicenseAsync();
        if (license.IsActive)
        {
            if (license.IsTrial)
            {
                licenseStatus = LicenseStatus.InTrial;

                int remainingTrialTime = (license.ExpirationDate - DateTime.Now).Days;

                if (remainingTrialTime <= 0)
                {
                    licenseStatus = LicenseStatus.TrialEnded;
                }
            }
            else
            {
                licenseStatus = LicenseStatus.Licensed;
            }
        }

        return licenseStatus;
    }

    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(nint hwnd);
    }
}

public enum PurchaseStatus
{
    Succeeded,
    AlreadyPurchased,
    Error
}