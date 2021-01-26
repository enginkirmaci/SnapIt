namespace SnapIt.ViewModels.DesignTime
{
    public class MainWindowDesignViewModel
    {
        public string ThemeTitle { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }
        public bool IsVersion3000MessageShown { get; set; }
        public bool IsPaneOpen { get; set; }

        public MainWindowDesignViewModel()
        {
            IsRunning = true;
            Status = "Stop";
            ThemeTitle = "Light";
            IsVersion3000MessageShown = true;
            IsPaneOpen = true;
        }
    }
}