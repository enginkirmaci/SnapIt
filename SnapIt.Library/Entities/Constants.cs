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
        public const string AppUrl = "snapscreen.app";
        public const string AppFeedbackUrl = "snapscreen.app/support";
        public const string AppPurchaseUrl = "snapscreen.app/purchase";
        public const string AppVersionCheckUrl = "dl.snapscreen.app/latest.json";
        public const string AppNewVersionUrl = "dl.snapscreen.app/setup_SnapScreen_{0}.exe";
        public const string AppPrivacyUrl = "snapscreen.app/features/privacy-policy";
        public static string CompanyName => Application.CompanyName;
        public const string CompanyUrl = "enginkirmaci.com";
        public const string TwitterUrl = "twitter.com/enginkirmaci";
        public const string GithubUrl = "github.com/enginkirmaci";
        public const string LinkedinUrl = "linkedin.com/in/enginkirmaci/";
        public const string MainRegion = "MainRegion";
    }
}