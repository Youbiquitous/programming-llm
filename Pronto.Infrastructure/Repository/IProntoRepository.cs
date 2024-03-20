///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Pronto.Infrastructure.Models;
using Pronto.Infrastructure.Models.Abstractions;

namespace Pronto.Infrastructure.Repository
{
    public interface IProntoRepository<C, E>
    {
        public int AddInitialEmbeddings(IList<(C chunk, E embeddedChunk)> embeddings);
    }
}
