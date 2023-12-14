using System.Text.Json;
using System.Windows.Media;

namespace SnapIt.Common.Converters.Json;

public class ColorToStringJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string hexString = reader.GetString();
        byte[] bytes = new byte[4];

        for (int i = 0; i < 4; i++)
        {
            bytes[i] = Convert.ToByte(hexString.Substring((i * 2) + 1, 2), 16);
        }

        //return new Color(bytes[0], bytes[1], bytes[2], bytes[3]);
        return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        string hexString = $"#{BitConverter.ToString([value.A, value.R, value.G, value.B]).Replace("-", "")}";
        writer.WriteStringValue(hexString);
    }
}