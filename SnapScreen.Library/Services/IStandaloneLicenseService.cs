using SnapScreen.Library.Entities;

namespace SnapScreen.Library.Services
{
    public interface IStandaloneLicenseService
    {
        bool VerifyLicenseKey(string key);

        LicenseStatus CheckStatus();

        StandaloneLicense License { get; }
    }
}