using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

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

    public static T Deserialize<T>(this string stringToDeserialize, bool camel = false) where T : new()
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(stringToDeserialize, camel ? camelCaseOptions : defaultOptions);

            if(result is null)
                return new();

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            try
            {
                var correctedString = TryToCorrectJsonString(stringToDeserialize);
                var result = JsonSerializer.Deserialize<T>(correctedString, camel ? camelCaseOptions : defaultOptions);

                if(result is null)
                    return new();

                return result;
            }
            catch (Exception exInner)
            {
                Console.WriteLine(exInner);
                return new();
            }
        }
    }

    private static string TryToCorrectJsonString(string jsonString)
    {
        int startIndex = jsonString.IndexOf('{');
        int endIndex = jsonString.LastIndexOf('}');

        if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
            throw new InvalidOperationException("A string JSON fornecida não contém um objeto JSON válido.");

        //Temp solution for chatGPT wrong generations
        string correctedString = jsonString[startIndex..(endIndex + 1)]
            .Replace(@"\n", @"\\n")
            .Replace(@"\\\n", @"\\n");

        return correctedString;
    }
}