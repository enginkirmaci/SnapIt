using WpfScreenHelper;

namespace SnapScreen.Library.Entities
{
    public class SnapScreen : Bindable
    {
        private Layout layout;
        private bool isActive = true;

        public bool IsActive
        { get => isActive; set { SetProperty(ref isActive, value); } }

        public string DeviceName { get; set; }
        public string Primary { get => Base.Primary ? "Primary" : null; }

        public string DeviceNumber { get => Base.DeviceName.Replace(@"\\.\DISPLAY", string.Empty); }
        public string Resolution { get => $"{Base.PixelBounds.Width} X {Base.PixelBounds.Height}"; }

        public Screen Base { get; set; }

        public Layout Layout
        { get => layout; set { SetProperty(ref layout, value); } }

        public System.Windows.Rect WorkingArea
        {
            get
            {
                return new System.Windows.Rect(
                    Base.WorkingArea.X * Base.ScaleFactor,
                    Base.WorkingArea.Y * Base.ScaleFactor,
                    Base.WorkingArea.Width * Base.ScaleFactor,
                    Base.WorkingArea.Height * Base.ScaleFactor);
            }
        }

        public SnapScreen(Screen screen, string devicePath)
        {
            Base = screen;

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