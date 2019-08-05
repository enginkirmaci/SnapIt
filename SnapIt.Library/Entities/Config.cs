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

		public bool DragByTitle { get; set; } = false;

		[JsonConverter(typeof(StringEnumConverter))]
		public MouseButton MouseButton { get; set; } = MouseButton.Left;

		public bool DisableForFullscreen { get; set; } = true;
		public Dictionary<string, string> ScreensLayouts { get; set; }
	}
}