using System;

namespace SnapScreen.Library.Entities
{
    public class ActiveWindow
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; }
        public Rectangle Boundry { get; set; }
        public Dpi Dpi { get; set; }

        public static readonly ActiveWindow Empty = new ActiveWindow();
    }
}