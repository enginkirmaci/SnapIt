using WpfScreenHelper;

namespace SnapIt.Library.Entities
{
    public class SnapScreen : Bindable
    {
        private Layout layout;
        private bool isActive = true;

        public bool IsActive { get => isActive; set { SetProperty(ref isActive, value); } }
        public string Primary { get => Base.Primary ? "Primary" : null; }

        public string DeviceNumber { get => Base.DeviceName.Replace(@"\\.\DISPLAY", string.Empty); }
        public string Resolution { get => $"{Base.Bounds.Width} X {Base.Bounds.Height}"; }

        public Screen Base { get; set; }

        public Layout Layout { get => layout; set { SetProperty(ref layout, value); } }

        public SnapScreen(Screen screen)
        {
            Base = screen;
        }
    }
}