using System.Windows.Forms;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Mappers
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

				case MouseButton.Left:
				default:
					return MouseButtons.Left;
			}
		}
	}
}