using System.Windows.Forms;

namespace SnapIt.UI.Resources
{
	public class Constants
	{
		public static readonly string AppLogo = "/SnapIt.UI;component/Themes/snapit.png";

		public static string AppName { get { return Application.ProductName; } }

		public static string AppVersion { get { return string.Format("version {0}", Application.ProductVersion); } }

		public static string AppUrl { get { return "enginkirmaci.com/projects/snapit"; } }

		public static string CompanyName { get { return Application.CompanyName; } }

		public static string CompanyUrl { get { return "enginkirmaci.com"; } }

		public static string TwitterUrl { get { return "twitter.com/enginkirmaci"; } }

		public static string GithubUrl { get { return "github.com/enginkirmaci"; } }

		public static string LinkedinUrl { get { return "linkedin.com/in/enginkirmaci/"; } }
	}
}