///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Pronto.Gpt.Prompts;
using Pronto.Gpt.Prompts.Core;
using Youbiquitous.Fluent.Gpt.Extensions;
using Youbiquitous.Fluent.Gpt.Providers;

namespace Youbiquitous.Fluent.Gpt;

/// <summary>
/// Main class
/// </summary>
public sealed class GptEmbeddingEngine
{
    private readonly string _apiKey;
    private string _baseUrl;
    private readonly IList<string> _inputs;
    private List<Prompt> _prompts;
    private string? _embeddingModel;
    private string? _manipulationModel;

    private (string Text, float[] Embedding) _result = (string.Empty, Array.Empty<float>());
    private GptEmbeddingEngine(string apiKey, string baseUrl)
    {
        _apiKey = apiKey;
        _baseUrl = baseUrl;
        _inputs = new List<string>();
        _prompts = new List<Prompt>();
    }

    /// <summary>
    /// Public factory method setting credentials
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="baseUrl"></param>
    /// <returns></returns>
    public static GptEmbeddingEngine Using(string apiKey, string baseUrl)
    {
        var e = new GptEmbeddingEngine(apiKey, baseUrl);
        return e;
    }


    /// <summary>
    /// Configure model
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public GptEmbeddingEngine ConversationalModel(string model)
    {
        _manipulationModel = model;
        return this;
    }

    /// <summary>
    /// Configure model
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public GptEmbeddingEngine EmbeddingModel(string model)
    {
        _embeddingModel = model;
        return this;
    }

    /// <summary>
    /// Add texy to embed
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public GptEmbeddingEngine Input(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return this;

        _inputs.Add(text.Replace('\n', ' '));
        return this;
    }

    /// <summary>
    /// Invokes the underlying API
    /// </summary>
    /// <returns></returns>
    public GptEmbeddingEngine Embed()
    {
        var client = new OpenAIClient(new Uri(_baseUrl), new AzureKeyCredential(_apiKey));
        var _embeddingInput = string.Join(' ', _inputs.ToArray());
        //Manipulate inputs according to prompts
        //Chaining togheter multiple manipulation steps
        foreach (var _prompt in _prompts)
        {
            var options = GptEngineOptions.Default();
            options.Temperature = 0;
            options.Messages.Add(new ChatRequestSystemMessage(_prompt.Build(_embeddingInput)));
            //Make the manipulation call
            var manipulationResponse = client.GetChatCompletions(options);
            if(manipulationResponse.HasValue)
                _embeddingInput = manipulationResponse.Value.Choices[0].Message.Content;
        }

        //Add inputs to embed
        var _options = new EmbeddingsOptions(_manipulationModel,new List<string> { _embeddingInput });

        //Make the embedding call
        var response = client.GetEmbeddings(_options);

        //Return embeddings
        if (response.HasValue)
            _result = (_embeddingInput, response.Value.Data[0].Embedding.ToArray());

        return this;
    }

    /// <summary>
    /// Returns the final output
    /// </summary>
    /// <returns></returns>
    public (string Text, float[] Embedding) GetResult()
    {
        return _result;
    }
}