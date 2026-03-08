using root.Models;

namespace root.Services
{
    public class VectorSearchService
    {
        public List<VectorItem> Search(
            float[] queryEmbeddings,
            List<VectorItem> items,
            int topK = 5)
        {
            return items
                .Select(i => new
                {
                    Item = i,
                    Score = CosineSimilarity(queryEmbeddings, i.Embedding)
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => x.Item)
                .ToList();
        }

        private float CosineSimilarity(float[] a, float[] b)
        {
            float magA = 0;
            float magB = 0;
            float dot = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            return dot / ((float)Math.Sqrt(magA) * (float)Math.Sqrt(magB));
        }
    }
}