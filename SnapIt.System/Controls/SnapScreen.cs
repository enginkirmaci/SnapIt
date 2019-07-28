using System.Windows.Forms;
using SnapIt.Entities;

namespace SnapIt.Controls
{
    public class SnapScreen : Bindable
    {
        private Layout layout;

        public string Primary { get => Base.Primary ? "Primary" : string.Empty; }
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