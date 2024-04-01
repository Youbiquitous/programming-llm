///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Identity.Client;
using Pronto.Gpt.Prompts.Core;
using Youbiquitous.Fluent.Gpt.Extensions;
using Youbiquitous.Fluent.Gpt.Providers;

namespace Youbiquitous.Fluent.Gpt;

/// <summary>
/// Main class
/// </summary>
public sealed class GptConversationalEngine
{
    private readonly string _apiKey;
    private readonly string _baseUrl;
    private readonly IList<ChatRequestMessage> _inputs;
    private Pronto.Gpt.Prompts.Core.Prompt _prompt;
    private IList<ChatRequestMessage> _history;
    private IContextProvider? _contextProvider;
    private ChatCompletionsOptions _options;
    private long? _seed = null;
    private string? _model;

    private GptConversationalEngine(string apiKey, string baseUrl)
    {
        _apiKey = apiKey;
        _baseUrl = baseUrl;
        _history = new List<ChatRequestMessage>();
        _inputs = new List<ChatRequestMessage>();
        _options = GptEngineOptions.Default();
    }

    /// <summary>
    /// Public factory method setting credentials
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public static GptConversationalEngine Using(string apiKey, string baseUrl)
    {
        var e = new GptConversationalEngine(apiKey, baseUrl);
        return e;
    }

    /// <summary>
    /// Configure model
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public GptConversationalEngine Model(string model)
    {
        _model = model;
        return this;
    }

    /// <summary>
    /// Add options
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    public GptConversationalEngine With(ChatCompletionsOptions options)
    {
        _options = options;
        return this;
    }

    /// <summary>
    /// Add prompt
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public GptConversationalEngine Prompt(Pronto.Gpt.Prompts.Core.Prompt prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt.Text))
            return this;

        _prompt = prompt;
        return this;
    }

    /// <summary>
    /// Add seed for reproducible output
    /// </summary>
    /// <param name="prompt"></param>
    /// <returns></returns>
    public GptConversationalEngine Seed(long seed)
    {
        _seed = seed;
        return this;
    }

    /// <summary>
    /// Add prompt
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public GptConversationalEngine User(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return this;

        _inputs.Add(new ChatRequestUserMessage(message));
        return this;
    }

    /// <summary>
    /// Add prompt
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public GptConversationalEngine Assistant(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return this;

        _inputs.Add(new ChatRequestAssistantMessage(message));
        return this;
    }

    /// <summary>
    /// Add history (as serialized)
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public GptConversationalEngine History(string messages)
    {
        if (string.IsNullOrWhiteSpace(messages))
            return this;

        _history = JsonSerializer.Deserialize<List<ChatRequestMessage>>(messages) ?? new List<ChatRequestMessage>();
        return this;
    }

    /// <summary>
    /// Add history (as list)
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public GptConversationalEngine History(IList<ChatRequestMessage> messages)
    {
        _history = messages; 
        return this;
    }

    /// <summary>
    /// Invokes the underlying API
    /// </summary>
    /// <returns></returns>
    public (Response<ChatCompletions>? Content, IList<ChatRequestMessage> History) Chat()
    {
        // First, add prompt messages (including few-shot examples)
        var promptMessages = _prompt.ToChatMessages();
        foreach (var m in promptMessages)
            _options.Messages.Add(m);

        // Then, add user input to history
        foreach (var m in _inputs)
            _history.Add(m);

        // Now, add relevant history (based on history window)
        var historyWindow = _prompt.HistoryWindow > 0
            ? _prompt.HistoryWindow
            : _history.Count;
        foreach (var m in _history.TakeLast(historyWindow))
            _options.Messages.Add(m);

        //Make the call(s)
        var client = new OpenAIClient(new Uri(_baseUrl), new AzureKeyCredential(_apiKey));
        _options.DeploymentName = _model;        
        //Make the call reproducible if needed (_seed null by default)
        _options.Seed = _seed;

        var response = client.GetChatCompletions(_options);

        //Update history
        if (response?.HasValue ?? false)
            _history.Add(new ChatRequestAssistantMessage(response.Value.Choices[0].Message));

        return (response, _history);
    }

    public async void Test()
    {
        var client = new OpenAIClient(new Uri(_baseUrl), new AzureKeyCredential(_apiKey));

        var getWeatherFunction = new ChatCompletionsFunctionToolDefinition();
        getWeatherFunction.Name = "GetWeather";
        getWeatherFunction.Description = "Use this tool when the user asks for weather information or forecasts, providing the city and the time frame of interest.";
        getWeatherFunction.Parameters = BinaryData.FromObjectAsJson(new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                ["WeatherInfoRequest"] = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["city"] = new JsonObject
                        {
                            ["type"] = "string",
                            ["description"] = @"The city the user wants to check the weather for."
                        },
                        ["startDate"] = new JsonObject
                        {
                            ["type"] = "date",
                            ["description"] = @"The start date the user is interested in for the weather forecast."
                        },
                        ["endDate"] = new JsonObject
                        {
                            ["type"] = "date",
                            ["description"] = @"The end date the user is interested in for the weather forecast."
                        }
                    },
                    ["required"] = new JsonArray { "city" }
                }
            },
            ["required"] = new JsonArray { "WeatherInfoRequest" }
        });
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = _model,
            Temperature = 0,
            MaxTokens = 1000,
            Tools = { getWeatherFunction },
            ToolChoice = ChatCompletionsToolChoice.Auto
        };
        // Completion Call
        var chatCompletionsResponse = await client.GetChatCompletionsAsync(chatCompletionsOptions);
        var llmResponse = chatCompletionsResponse.Value.Choices.FirstOrDefault();
        // See if as a response ChatGPT wants to call a function
        if (llmResponse.FinishReason == CompletionsFinishReason.ToolCalls)
        {
            //  To allow GPT  to call multiple sequential functions
            bool functionCallingComplete = false;
            while (!functionCallingComplete)
            {
                // Add the assistant message with tool calls to the conversation history
                ChatRequestAssistantMessage toolCallHistoryMessage = new(llmResponse.Message);
                chatCompletionsOptions.Messages.Add(toolCallHistoryMessage);

                //  To allow GPT  to call multiple parallel functions
                foreach (ChatCompletionsToolCall functionCall in llmResponse.Message.ToolCalls)
                {
                    // Get the arguments
                    var functionArgs = ((ChatCompletionsFunctionToolCall)functionCall).Arguments;
                    
                    // Variable to hold the function result
                    string functionResult = "";
                    //Calling the function deserializing
                    //var weatherInfoRequest = JsonSerializer.Deserialize<WeatherInfoRequest>(functionArgs);
                    //if (weatherInfoRequest != null)
                    //{
                        //functionResult = GetWeather(weatherInfoRequest);
                    //}
                    // Add function message
                    //needed to tell the model about the function output and have it to elaborate a final answer for the user 
                    var chatFunctionMessage = new ChatRequestToolMessage(functionResult, functionCall.Id);
                    chatCompletionsOptions.Messages.Add(chatFunctionMessage);
                }

                //One more Chat Completion call to see what’s next
                var innerCompletionCall = client.GetChatCompletionsAsync(chatCompletionsOptions).Result.Value.Choices.FirstOrDefault();

                // Create a new Message object with the response and add it to the messages list
                if (innerCompletionCall.Message != null)
                {
                    chatCompletionsOptions.Messages.Add(new ChatRequestAssistantMessage(innerCompletionCall.Message));
                }
                // Break out of the loop
                if (innerCompletionCall.FinishReason != CompletionsFinishReason.ToolCalls)
                {
                    functionCallingComplete = true;
                }
            }
        }
    }

    public Task<StreamingResponse<StreamingChatCompletionsUpdate>>? ChatStreaming()
    {
        // First, add prompt messages (including few-shot examples)
        var promptMessages = _prompt.ToChatMessages();
        foreach (var m in promptMessages)
            _options.Messages.Add(m);

        // Then, add user input to history
        foreach (var m in _inputs)
            _history.Add(m);

        // Now, add relevant history (based on history window)
        var historyWindow = _prompt.HistoryWindow > 0 
            ? _prompt.HistoryWindow 
            : _history.Count;
        foreach (var m in _history.TakeLast(historyWindow))
            _options.Messages.Add(m);

        //Make the call(s)
        var client = new OpenAIClient(new Uri(_baseUrl), new AzureKeyCredential(_apiKey));
        _options.DeploymentName = _model;
        //Make the call reproducible if needed (_seed null by default)
        _options.Seed = _seed;

        var response = client.GetChatCompletionsStreamingAsync(_options);

        return response;
    }
}