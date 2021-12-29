namespace SnapScreen.ViewModels.DesignTime
{
    public class MainWindowDesignViewModel
    {
        public string ThemeTitle { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }
        public bool IsTrial { get; set; }
        public string LicenseText { get; set; }

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