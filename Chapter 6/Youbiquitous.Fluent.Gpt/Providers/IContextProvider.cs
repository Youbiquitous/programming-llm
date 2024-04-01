///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Azure.AI.OpenAI;
using System.Collections.Generic;
using Youbiquitous.Fluent.Gpt.Model;

namespace Youbiquitous.Fluent.Gpt.Providers;

/// <summary>
/// Main class
/// </summary>
public interface IContextProvider
{
    /// <summary>
    /// Name of the provider
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Retrieve the related context for a given chat message
    /// </summary>
    /// <returns></returns>
    public IList<ContextPiece> GetContext(IList<ChatRequestUserMessage> messages);
}

