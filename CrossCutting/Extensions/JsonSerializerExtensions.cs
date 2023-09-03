using System.Text.Json.Serialization;
using System.Text.Json;

namespace CrossCutting.Extensions;

public static class JsonSerializerExtensions
{
    private static readonly JsonSerializerOptions camelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly JsonSerializerOptions defaultOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Serialize<T>(this T objectToSerialize) => JsonSerializer.Serialize(objectToSerialize, camelCaseOptions);

    public static T Deserialize<T>(this string stringToDeserialize, bool camel = false) =>
        JsonSerializer.Deserialize<T>(stringToDeserialize, camel ? camelCaseOptions : defaultOptions);
}