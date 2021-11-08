using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface IStandaloneLicenseService
    {
        bool VerifyLicenseKey(string key);

        LicenseStatus CheckStatus();

        StandaloneLicense License { get; }
    }
}