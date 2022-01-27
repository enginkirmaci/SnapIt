using System.Windows;

namespace SnapIt.Library.Entities
{
    public class SnapScreen : Bindable
    {
        private Layout layout;
        private bool isActive = true;
        private string primary;
        private bool isPrimary = false;
        private string deviceNumber;
        private string resolution;

        public string DeviceName { get; set; }
        public Rect WorkingArea { get; set; }
        public Rect PixelBounds { get; set; }
        public Rect Bounds { get; set; }

        public bool IsActive
        { get => isActive; set { SetProperty(ref isActive, value); } }

        public bool IsPrimary
        { get => isPrimary; set { SetProperty(ref isPrimary, value); } }

        public string Primary
        { get => primary; set { SetProperty(ref primary, value); } }

        public string DeviceNumber
        { get => deviceNumber; set { SetProperty(ref deviceNumber, value); } }

        public string Resolution
        { get => resolution; set { SetProperty(ref resolution, value); } }

        public Layout Layout
        { get => layout; set { SetProperty(ref layout, value); } }

        public SnapScreen()
        { }

        public SnapScreen(WpfScreenHelper.Screen screen, string devicePath)
        {
            IsPrimary = screen.Primary;
            Primary = IsPrimary ? "Primary" : "";
            DeviceNumber = screen.DeviceName.Replace(@"\\.\DISPLAY", string.Empty);
            Resolution = $"{screen.PixelBounds.Width} X {screen.PixelBounds.Height}";

            WorkingArea = new Rect(
                    screen.WorkingArea.X * screen.ScaleFactor,
                    screen.WorkingArea.Y * screen.ScaleFactor,
                    screen.WorkingArea.Width * screen.ScaleFactor,
                    screen.WorkingArea.Height * screen.ScaleFactor);

            PixelBounds = screen.PixelBounds;
            Bounds = screen.Bounds;
            if (!string.IsNullOrEmpty(devicePath))
            {
                DeviceName = devicePath;
            }
            else
            {
                DeviceName = screen.DeviceName;
            }
        }
    }
}