namespace SnapIt.ViewModels.DesignTime
{
    public class MainWindowDesignViewModel
    {
        public string ThemeTitle { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }
        public bool IsTrial { get; set; }
        public bool IsVersion3000MessageShown { get; set; }
        public bool IsPaneOpen { get; set; }
        public string LicenseText { get; set; }

        public MainWindowDesignViewModel()
        {
            IsTrial = true;
            IsRunning = true;
            Status = "Stop";
            ThemeTitle = "Light";
            IsVersion3000MessageShown = true;
            IsPaneOpen = true;
            LicenseText = "licensed to Engin KIRMACI";
        }
    }
}