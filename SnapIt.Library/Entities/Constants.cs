using System.Windows.Forms;

namespace SnapIt.Library.Entities
{
    public class Constants
    {
        public const string AppStoreId = "9PHGBMZ7RBZX";
        public const string AppLogo = "/SnapIt.UI;component/Themes/snapit.png";
#if !STANDALONE
        public static string AppName => Application.ProductName;
#endif

#if STANDALONE
        public static string AppName => $"{Application.ProductName} Pro";
#endif
        public static string AppTitle => $"{AppName} - Window Manager";
        public static string AppVersion => $"version {Application.ProductVersion}";
        public const string AppUrl = "getsnapit.com";
        public const string AppFeedbackUrl = "getsnapit.com/support";
        public const string AppPurchaseUrl = "getsnapit.com/checkout";
        public const string AppVersionCheckUrl = "dl.getsnapit.com/latest.json";
        public const string AppNewVersionUrl = "dl.getsnapit.com/setup_SnapScreenPro_{0}.exe";
        public const string AppPrivacyUrl = "getsnapit.com/features/privacy-policy";
        public const string AppRegistryKey = "SnapItPro";
        public static string CompanyName => Application.CompanyName;
        public const string CompanyUrl = "enginkirmaci.com";
        public const string TwitterUrl = "twitter.com/enginkirmaci";
        public const string GithubUrl = "github.com/enginkirmaci";
        public const string LinkedinUrl = "linkedin.com/in/enginkirmaci/";
        public const string MainRegion = "MainRegion";
    }
}