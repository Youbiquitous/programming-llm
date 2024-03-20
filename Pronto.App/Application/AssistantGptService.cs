///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;
using Pronto.Shared.Settings;
using Pronto.App.Models;
using Pronto.Gpt.Prompts;
using Youbiquitous.Fluent.Gpt;
using Microsoft.Extensions.Primitives;
using Youbiquitous.Fluent.Gpt.Providers;
using Pronto.Gpt.Prompts.Core;
using System.Linq;

namespace Pronto.App.Application;

public class AssistantGptService : ApplicationServiceBase
{
    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="settings"></param>
    public AssistantGptService(AppSettings settings) 
        : base(settings)
    {
    }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="history"></param>
    /// <param name="queue"></param>
    public ChatViewModel GetChatViewModel(IList<ChatRequestMessage> history, string queue)
    {
        return new ChatViewModel(Settings, history, queue);
    }

    /// <summary>
    /// Ask GPT to process the message
    /// </summary>
    /// <param name="originalEmail"></param>
    /// <param name="message"></param>
    /// <param name="history"></param>
    /// <returns></returns>
    public async Task<StreamingResponse<StreamingChatCompletionsUpdate>> HandleStreamingMessage(string message, IList<ChatRequestMessage> history, string originalEmail = "")
    {
        //If it's not the first message, we don't need to re-enter original email and first draft
        if (history.Any())
        {
            var response = await GptConversationalEngine
                .Using(Settings.General.OpenAI.ApiKey, Settings.General.OpenAI.BaseUrl)
                .Model(Settings.General.OpenAI.DeploymentId)
                .Prompt(new AssistantPrompt())
                .History(history)
                .User(message)
                .ChatStreaming();

            return response;
        }
        //If it's the first message, we need to send the original email + the draft answer
        else
        {
            var response = await GptConversationalEngine
                .Using(Settings.General.OpenAI.ApiKey, Settings.General.OpenAI.BaseUrl)
                .Model(Settings.General.OpenAI.DeploymentId)
                .Prompt(new AssistantPrompt())
                .History(history)
                .User("Original Email: " + originalEmail)
                .User("Engineers' Answer: " + message)
                .ChatStreaming();

            return response;
        }
    }

    /// <summary>
    /// Ask GPT to translate the email
    /// </summary>
    /// <param name="email"></param>
    /// <param name="destinationLanguage"></param>
    /// <returns></returns>
    public string Translate(string email, string destinationLanguage)
    {
        var response = GptConversationalEngine
            .Using(Settings.General.OpenAI.ApiKey, Settings.General.OpenAI.BaseUrl)
            .Model(Settings.General.OpenAI.DeploymentId)
            .With(new ChatCompletionsOptions() { Temperature = 0 })
            .Prompt(new TranslationPrompt(destinationLanguage))
            .Seed(42)
            .User(email)
            .Chat();

        //returning the original text if GPT fails
        return response.Content.HasValue
                    ? response.Content.Value.Choices[0].Message.Content
                    : email;
    }
}