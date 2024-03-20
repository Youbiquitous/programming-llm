///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

namespace Pronto.Infrastructure.Models.Abstractions;

public partial interface IEmbedding
{
    public long Id { get; set; }
    public long ChunkId { get; set; }

    public int VectorValueId { get; set; }

    public double VectorValueContent { get; set; }
}