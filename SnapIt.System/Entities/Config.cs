using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SnapIt.Entities
{
    public class Config
    {
        public bool DragByTitle { get; set; } = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public MouseButton MouseButton { get; set; } = MouseButton.Left;
    }
}