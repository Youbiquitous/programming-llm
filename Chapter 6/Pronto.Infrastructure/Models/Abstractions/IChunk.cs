///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

namespace Pronto.Infrastructure.Models.Abstractions;

public partial interface IChunk
{
    public long Id { get; set; }

    public string Content { get; set; }
}