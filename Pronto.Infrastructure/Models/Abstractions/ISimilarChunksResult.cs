///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

namespace Pronto.Infrastructure.Models.Abstractions;

public partial interface ISimilarChunksResult
{
    public int Id { get; set; }
    public string Content { get; set; }
    public double? CosineDistance { get; set; }
}
