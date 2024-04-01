///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;

namespace Youbiquitous.Fluent.Gpt;

public static class GptEngineOptions
{
    /// <summary>
    /// Default GPT options to use
    /// </summary>
    /// <returns></returns>
    public static ChatCompletionsOptions Default()
    {
        return new ChatCompletionsOptions()
        {
            Temperature = (float)0.7,
            MaxTokens = 800,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };
    }
}