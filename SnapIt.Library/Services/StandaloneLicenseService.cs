using System;
using SnapIt.Library.Entities;
using ThinkSharp.Licensing;

namespace SnapIt.Library.Services
{
    public enum LicenseStatus
    {
        InTrial,
        TrialEnded,
        Licensed
    }

    public class StandaloneLicenseService : IStandaloneLicenseService
    {
        private readonly ISettingService settingService;
        private static string publicKey = "BgIAAACkAABSU0ExAAQAAAEAAQCBY+E7Jzq11V5MZ0uq7ti3iFb/6X/z9R6G4oaPIZyUQN1Oxz650QICctnnRjOsME2uCicInqqGEC+X/HJhH8e/YZtRdqt1Bb2rNnP/TDhLdUEOb6V9fOUWsGqjQ8u6BTaIX/0bSUZp26IHkJMpynQdyPEta/b0D7XnA/ORFJzdnQ==";
        public StandaloneLicense License { get; private set; }

        public StandaloneLicenseService(
            ISettingService settingService)
        {
            this.settingService = settingService;
        }

        public bool VerifyLicenseKey(string key)
        {
            try
            {
                var signedLicense = Lic.Verifier
                     .WithRsaPublicKey(publicKey)
                     .WithApplicationCode("SSQ")
                     .LoadAndVerify(key);

                License.Name = signedLicense.Properties["Name"];
                License.IsValid = true;
                License.Value = signedLicense.Serialize();
                License.CreatedDate = DateTime.Now;

                settingService.SaveStandaloneLicense(License);

                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        public LicenseStatus CheckStatus()
        {
            License = settingService.StandaloneLicense;
            var licenseStatus = LicenseStatus.InTrial;

            var diffDate = DateTime.Now.Subtract(License.CreatedDate).TotalDays % 30;

            if (!License.IsValid && License.CreatedDate.AddDays(31) > DateTime.Now)
            {
                License.IsValid = false;
                licenseStatus = LicenseStatus.InTrial;
            }
            else if (License.IsValid && (int)diffDate == 0)
            {
                try
                {
                    var signedLicense = Lic.Verifier
                         .WithRsaPublicKey(publicKey)
                         .WithApplicationCode("SSQ")
                         .LoadAndVerify(License.Value);

                    License.Name = signedLicense.Properties["Name"];
                    License.IsValid = true;
                    licenseStatus = LicenseStatus.Licensed;
                }
                catch (Exception)
                {
                    License.IsValid = false;
                    licenseStatus = LicenseStatus.TrialEnded;
                }
            }
            else if (License.IsValid)
            {
                License.IsValid = true;
                licenseStatus = LicenseStatus.Licensed;
            }
            else
            {
                License.IsValid = false;
                licenseStatus = LicenseStatus.TrialEnded;
            }

            settingService.SaveStandaloneLicense(License);

            return licenseStatus;
        }
    }
}