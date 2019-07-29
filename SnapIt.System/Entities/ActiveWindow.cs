using System;

namespace SnapIt.Entities
{
	public class ActiveWindow
	{
		public IntPtr Handle { get; set; }
		public string Title { get; set; }

		public static readonly ActiveWindow Empty = new ActiveWindow();
	}
}