///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Azure.AI.OpenAI;
using Pronto.Infrastructure.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Youbiquitous.Fluent.Gpt.Model;

namespace Youbiquitous.Fluent.Gpt.Providers;

/// <summary>
/// Gets context from a SQL database (low performances)
/// Ref: https://devblogs.microsoft.com/azure-sql/vector-similarity-search-with-azure-sql-database-and-openai/
/// </summary>
public class SqlContextProvider : IContextProvider
{
    private GptEmbeddingEngine _embeddingEnging;

    /// <summary>
    /// CTOR
    /// </summary>
    public SqlContextProvider()
    {
        
    }
    /// <summary>
    /// A well-configuered GptEmbedding engine must be provided
    /// </summary>
    public SqlContextProvider(GptEmbeddingEngine configuredEmbeddingEngine)
    {
        Name = "SQL";
        _embeddingEnging = configuredEmbeddingEngine;
    }

    /// <summary>
    /// Name of the provider
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Retrieve the context on the provided SQL database
    /// </summary>
    /// <returns></returns>
    public IList<ContextPiece> GetContext(IList<ChatRequestUserMessage> messages)
    {
        using var db = new ProntoContext();
        var embedding = _embeddingEnging.Input(string.Join(" ", messages.Select(m => m.Content))).Embed().GetResult().Embedding;
        //Format as json array for the SQL function
        var vectorData = "[" + string.Join(",", embedding) + "]";
        
        var similarChunks = db.SimilarChunks(vectorData, 5);
        return similarChunks
            .Select(sim => new ContextPiece { Text = sim.Content, Reference = sim.Id.ToString() })
            .ToList();
    }
}
