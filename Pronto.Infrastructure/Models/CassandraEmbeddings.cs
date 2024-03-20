///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Pronto.Infrastructure.Models.Abstractions;

namespace Pronto.Infrastructure.Models;

public partial class CassandraEmbedding:IEmbedding
{
    public long Id { get; set; }
    public long ChunkId { get; set; }

    public int VectorValueId { get; set; }

    public double VectorValueContent { get; set; }
}