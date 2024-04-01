///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Pronto.Infrastructure.Models.Abstractions;

namespace Pronto.Infrastructure.Models;

public partial class CassandraChunk:IChunk
{
    public CassandraChunk()
    {
    }
    public CassandraChunk(string content)
    {
        Content = content;
    }

    public long Id { get; set; }

    public string Content { get; set; }
}