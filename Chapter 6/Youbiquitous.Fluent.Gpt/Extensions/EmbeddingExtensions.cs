using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Youbiquitous.Fluent.Gpt.Extensions
{
    public static class EmbeddingExtensions
    {
        public static string ToString(this float[] embedding)
        {
            //return to a SQL useful form
            return "[" + string.Join(",", embedding) + "]";
        }
    }
}
