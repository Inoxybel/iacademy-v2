using Newtonsoft.Json;
using Service.Integrations.OpenAI.DTO;

namespace Service.Integrations.OpenAI.Extensions;

public static class OpenAIResponseExtensions
{
    public static T MapTopicsFromResponse<T>(this OpenAIResponse response) where T : class, new()
    {
        var completeResponse = response.Choices.First().Message.Content;

        var startIndex = completeResponse.IndexOf('{');
        var endIndex = completeResponse.LastIndexOf('}');

        if (startIndex != -1 && endIndex != -1)
        {
            var extractedJson = completeResponse.Substring(startIndex, endIndex - startIndex + 1);

            var objectFromResponse = JsonConvert.DeserializeObject<T>(extractedJson);

            if (objectFromResponse is not null)
                return objectFromResponse;
        }

        return new T();
    }
}