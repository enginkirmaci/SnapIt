using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SnapIt.Library.Entities
{
	public class Config
	{
		public Config()
		{
			ScreensLayouts = new Dictionary<string, string>();
		}

		public bool EnableMouse { get; set; } = true;

		public bool DragByTitle { get; set; } = true;

		[JsonConverter(typeof(StringEnumConverter))]
		public MouseButton MouseButton { get; set; } = MouseButton.Left;

		public bool EnableKeyboard { get; set; } = true;

		public bool DisableForFullscreen { get; set; } = true;
		public bool ShowMainWindow { get; set; } = true;
		public Dictionary<string, string> ScreensLayouts { get; set; }
	}
}