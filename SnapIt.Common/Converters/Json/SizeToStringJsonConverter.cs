using System.Text.Json;
using SnapIt.Common.Graphics;

namespace SnapIt.Common.Converters.Json;

public class SizeToStringJsonConverter : JsonConverter<Size>
{
    public override Size? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string[] parts = reader.GetString().Split(',');
        return new Size
        {
            Width = float.Parse(parts[0]),
            Height = float.Parse(parts[1])
        };
    }

    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{value.Width},{value.Height}");
    }
}