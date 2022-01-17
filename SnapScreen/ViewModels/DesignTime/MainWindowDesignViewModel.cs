namespace SnapScreen.ViewModels.DesignTime
{
    public class MainWindowDesignViewModel
    {
        public string ThemeTitle { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }
        public bool IsTrial { get; set; }
        public string LicenseText { get; set; }
        public bool IsTrialMessageOpen { get; set; } = false;
        public bool IsLicenseMessageOpen { get; set; } = false;
        public string LicenseMessageErrorText { get; set; } = "Error";
        public bool IsLicenseSuccess { get; set; } = false;
        public bool NewVersionMessageOpen { get; set; } = false;
        public bool IsTryStoreMessageOpen { get; set; } = false;

        public MainWindowDesignViewModel()
        {
            IsTrial = true;
            IsRunning = true;
            Status = "Stop";
            ThemeTitle = "Light";
            LicenseText = "licensed to Engin KIRMACI";
        }
    }
}