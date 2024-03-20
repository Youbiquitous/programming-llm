///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using System.Collections.Generic;
using Azure.AI.OpenAI;

namespace Youbiquitous.Fluent.Gpt.Providers;

/// <summary>
/// Main class
/// </summary>
public interface IHistoryProvider<T>
{
    /// <summary>
    /// Name of the provider
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Retrieve the stored list of chat messages
    /// </summary>
    /// <returns></returns>
    IList<ChatRequestMessage> GetMessages(T userId);

    /// <summary>
    /// Save a new list of chat messages
    /// </summary>
    /// <returns></returns>
    bool SaveMessages(IList<ChatRequestMessage> messages, T userId);
}