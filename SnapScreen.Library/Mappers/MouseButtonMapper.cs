using System.Windows.Forms;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.Mappers
{
    public class MouseButtonMapper
    {
        public static MouseButtons Map(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Right:
                    return MouseButtons.Right;

                case MouseButton.Middle:
                    return MouseButtons.Middle;

                case MouseButton.XButton1:
                    return MouseButtons.XButton1;

                case MouseButton.XButton2:
                    return MouseButtons.XButton2;

                case MouseButton.Left:
                default:
                    return MouseButtons.Left;
            }
        }
    }
}