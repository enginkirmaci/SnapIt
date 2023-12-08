using System.Text.Json;
using SnapIt.Common.Converters.Json;

namespace SnapIt.Common.Entities;

public class JsonOptions
{
    private static JsonSerializerOptions defaultOptions = null;

    public static JsonSerializerOptions DefaultOptions
    {
        get
        {
            if (defaultOptions == null)
            {
                defaultOptions = new JsonSerializerOptions
                {
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    IgnoreReadOnlyProperties = true,
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
                };

                defaultOptions.Converters.Add(new JsonStringEnumConverter());
                defaultOptions.Converters.Add(new SizeToStringJsonConverter());
                defaultOptions.Converters.Add(new PointToStringJsonConverter());
                defaultOptions.Converters.Add(new ColorToStringJsonConverter());
            }

            return defaultOptions;
        }
    }
}