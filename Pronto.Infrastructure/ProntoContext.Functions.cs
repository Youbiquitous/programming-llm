using Microsoft.EntityFrameworkCore;

///////You first need to create a table valued function on SQL
///
//CREATE function[dbo].[SimilarChunks] (@vector nvarchar(max), @maxResults int)
//returns table
//as
//return with cteVector as
//(
//    select
//        cast([key] as int) as [Vector_Value_Id],
//        cast([value] as float) as [Vector_Value_Content]
//        from
//        openjson(@vector)
//),
//cteSimilar as
//(
//select top (@maxResults)
//    v2.ChunkId,
//    sum(v1.[Vector_Value_Content]* v2.[Vector_Value_Content]) as CosineDistance
//from
//    cteVector v1
//inner join
//    dbo.Embeddings v2 on v1.Vector_Value_Id = v2.Vector_Value_Id
//group by
//    v2.ChunkId
//order by
//    CosineDistance desc
//)
//select
//    a.Id,
//    a.Content,
//    r.CosineDistance
//from
//    cteSimilar r
//inner join
//    dbo.Chunks a on r.ChunkId = a.Id

namespace Pronto.Infrastructure.Models
{
    public partial class ProntoContext
    {
        [DbFunction("SimilarChunks", "dbo")]
        public IQueryable<SimilarChunksResult> SimilarChunks(string vector, int? maxResults)
        {
            return FromExpression(() => SimilarChunks(vector, maxResults));
        }

        protected void OnModelCreatingGeneratedFunctions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimilarChunksResult>().HasNoKey();
        }
    }
}
