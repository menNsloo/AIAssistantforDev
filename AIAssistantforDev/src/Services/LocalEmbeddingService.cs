using System.Text;
using System.Text.Json;

namespace root.Services
{
    public class LocalEmbeddingService
    {
        private readonly HttpClient _httpClient;

        public LocalEmbeddingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float[]> CreateEmbeddingAsync(string text)
        {
            var requestBody = new
            {
                model = "nomic-embed-text",
                input = new[] { text }
            };

            var json = JsonSerializer.Serialize(requestBody);

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/embed",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);

            var embeddingArray = doc.RootElement
                .GetProperty("embeddings")[0]
                .EnumerateArray()
                .Select(x => x.GetSingle())
                .ToArray();

            return embeddingArray;
        }
    }
}