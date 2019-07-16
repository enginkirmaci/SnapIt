using System.Windows.Forms;

namespace SnapIt.Entities
{
    public class SnapScreen
    {
        public string Primary { get => Base.Primary ? "Primary" : string.Empty; }
        public string DeviceNumber { get => Base.DeviceName.Replace(@"\\.\DISPLAY", string.Empty); }
        public string Resolution { get => $"{Base.Bounds.Width} X {Base.Bounds.Height}"; }
        public Screen Base { get; set; }

        public SnapScreen(Screen screen)
        {
            Base = screen;
        }
    }
}