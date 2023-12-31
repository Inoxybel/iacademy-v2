﻿using System.Text;
using CrossCutting.Extensions;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Service.Integrations.OpenAI.DTO;
using Service.Integrations.OpenAI.Interfaces;
using Service.Integrations.OpenAI.Options;

namespace Service.Integrations.OpenAI;

public class OpenAIService : IOpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsSnapshot<OpenAIOptions> _options;
    private const string URI = "v1/chat/completions";
    private const int RETRY_COUNT = 1;

    public OpenAIService(
        HttpClient httpClient,
        IOptionsSnapshot<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<OpenAIResponse> DoRequest(string objStringified)
    {
        var objRequest = new OpenAIRequest()
        {
            Model = _options.Value.Model,
            Messages = new List<Message>()
                {
                    new Message()
                    {
                        Role = "user",
                        Content = objStringified
                    }
                },
            Temperature = 0.8
        };

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        var request = JsonConvert.SerializeObject(objRequest, settings);

        var response = await ExecuteWithRetryPolicy(async () =>
        {
            var jsonContent = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(URI, jsonContent);

            return response;
        }, RETRY_COUNT);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var openIAResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);
            return openIAResponse;
        }

        return new OpenAIResponse();
    }

    public async Task<OpenAIResponse> DoRequest(ChatCompletion chatCompletion, string prompt)
    {
        var objRequest = new OpenAIRequest()
        {
            Model = _options.Value.Model,
            Messages = chatCompletion.Choices.Select(choice => new Message 
                { 
                    Role = choice.Message.Role, 
                    Content = choice.Message.Content 
                }
            ).ToList(),
            Temperature = 0.7,
            MaxTokens = 7000
        };

        objRequest.Messages.Add(new Message()
        {
            Role = "user",
            Content = prompt
        });

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        var request = JsonConvert.SerializeObject(objRequest, settings);

        var response = await ExecuteWithRetryPolicy(async () =>
        {
            var jsonContent = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(URI, jsonContent);

            return response;
        }, RETRY_COUNT);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var openIAResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);
            return openIAResponse;
        }

        return new OpenAIResponse();
    }

    private static async Task<HttpResponseMessage> ExecuteWithRetryPolicy(Func<Task<HttpResponseMessage>> action, int attempts)
    {
        var retryPolicy = Policy.Handle<Exception>()
            .OrResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode && !response.StatusCode.Is4XX())
            .WaitAndRetryAsync(
                retryCount: attempts,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );

        var response = await retryPolicy.ExecuteAsync(action);

        return response;
    }
}