namespace SnapIt.Library.Entities
{
    public class Constants
    {
        public const string AppStoreId = "9PHGBMZ7RBZX";
        public const string AppLogo = "/SnapIt.UI;component/Themes/snapit.png";
        public static string AppName => global::System.Windows.Forms.Application.ProductName;
        public static string AppTitle => $"{AppName} - Window Manager";
        public static string AppVersion => $"version {(global::System.Windows.Forms.Application.ProductVersion)}";
        public const string AppUrl = "getsnapit.com";
        public const string AppFeedbackUrl = "getsnapit.com/support";
        public const string AppPurchaseUrl = "getsnapit.com/checkout";
        public const string AppVersionCheckUrl = "github.com/enginkirmaci/SnapIt/blob/main/latest-version.json";
        public const string AppNewVersionUrl = "github.com/enginkirmaci/SnapIt/releases/download/{0}/setup_SnapIt_{0}.exe";
        public const string AppPrivacyUrl = "getsnapit.com/features/privacy-policy";
        public const string AppRegistryKey = "SnapIt";
        public static string CompanyName => global::System.Windows.Forms.Application.CompanyName;
        public const string CompanyUrl = "enginkirmaci.com";
        public const string TwitterUrl = "twitter.com/enginkirmaci";
        public const string GithubUrl = "github.com/enginkirmaci";
        public const string LinkedinUrl = "linkedin.com/in/enginkirmaci/";
        public const string MainRegion = "MainRegion";
    }
}