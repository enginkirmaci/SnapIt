using System.Text.Json;
using SnapIt.Common.Entities;

namespace SnapIt.Common.Helpers;

public static class Json
{
    public static T Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>(value, JsonOptions.DefaultOptions);
    }

    public static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, JsonOptions.DefaultOptions);
    }
}