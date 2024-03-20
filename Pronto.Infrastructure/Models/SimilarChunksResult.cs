///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

namespace Pronto.Infrastructure.Models
{
    public partial class SimilarChunksResult
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public double? CosineDistance { get; set; }
    }
}
