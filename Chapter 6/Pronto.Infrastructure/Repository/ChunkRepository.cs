///////////////////////////////////////////////////////////////////
//
// Internal GPT wrapper API
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using Microsoft.EntityFrameworkCore;
using Pronto.Infrastructure.Models;
using Pronto.Infrastructure.Models.Abstractions;

namespace Pronto.Infrastructure.Repository
{
    public class ChunkRepository : IProntoRepository<Chunk, float[]>
    {
        /// <summary>
        /// DB initialization to load all embedding for every chunk
        /// </summary>
        /// <returns></returns>
        public int AddInitialEmbeddings(IList<(Chunk chunk, float[] embeddedChunk)> embeddings)
        {
            using var _context = new ProntoContext();
            var ret = 0;
            try
            {
                foreach (var embedding in embeddings)
                {
                    _context.Chunks.Add(embedding.chunk);
                    _context.SaveChanges();

                    // Instert all embedding components
                    for (int i = 0; i < embedding.embeddedChunk.Length; i++)
                    {
                        var embeddingVector = new CassandraEmbedding
                        {
                            ChunkId = embedding.chunk.Id,
                            VectorValueId = i,
                            VectorValueContent = embedding.embeddedChunk[i]
                        };
                        _context.Embeddings.Add(embeddingVector);
                    }
                    ret += _context.SaveChanges();
                }
                return ret;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return ret;
            }
        }

        /// <summary>
        /// Add chunk + its embedding
        /// </summary>
        /// <returns></returns>
        public int AddEmbedding(ProntoContext _context, (Chunk chunk, float[] embeddedChunk) embedding)
        {
            _context.Chunks.Add(embedding.chunk);
            _context.SaveChanges();

            // Instert all embedding components
            for (int i = 0; i < embedding.embeddedChunk.Length; i++)
            {
                var embeddingVector = new CassandraEmbedding
                {
                    ChunkId = embedding.chunk.Id,
                    VectorValueId = i,
                    VectorValueContent = embedding.embeddedChunk[i]
                };
                _context.Embeddings.Add(embeddingVector);
            }
            return _context.SaveChanges();
        }


        /// <summary>
        /// Add chunk + its embedding
        /// </summary>
        /// <returns></returns>
        public int AddEmbedding((Chunk chunk, float[] embeddedChunk) embedding)
        {
            using var _context = new ProntoContext();
            _context.Chunks.Add(embedding.chunk);
            _context.SaveChanges();

            // Instert all embedding components
            for (int i = 0; i < embedding.embeddedChunk.Length; i++)
            {
                var embeddingVector = new CassandraEmbedding
                {
                    ChunkId = embedding.chunk.Id,
                    VectorValueId = i,
                    VectorValueContent = embedding.embeddedChunk[i]
                };
                _context.Embeddings.Add(embeddingVector);
            }
            return _context.SaveChanges();
        }


        /// <summary>
        /// Delete chunk + its embedding
        /// </summary>
        /// <returns></returns>
        public int DeleteChunk(int id)
        {
            using var _context = new ProntoContext();
            var article = _context.Chunks.Find(id);

            if (article == null) 
                return 1;
            
            _context.Chunks.Remove(article);
            _context.Embeddings.Where(e => e.ChunkId == id).ExecuteDelete();

            return _context.SaveChanges();
        }
    }
}
