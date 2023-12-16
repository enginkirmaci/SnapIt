using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;

namespace SnapIt.Services.Contracts;

public interface IStandaloneLicenseService : IInitialize
{
    bool VerifyLicenseKey(string key);

    LicenseStatus CheckStatus();

    StandaloneLicense License { get; }
}