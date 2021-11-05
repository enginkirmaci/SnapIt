using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface IStandaloneLicenseService
    {
        LicenseStatus CheckStatus();

        StandaloneLicense License { get; }
    }
}