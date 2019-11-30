namespace SnapIt.ViewModels.DesignTime
{
    public class MainWindowDesignViewModel
    {
        public string ThemeTitle { get; set; }
        public bool IsDarkTheme { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }

        public MainWindowDesignViewModel()
        {
            IsRunning = true;
            Status = "Stop";
            ThemeTitle = "Light";
            IsDarkTheme = false;
        }
    }
}