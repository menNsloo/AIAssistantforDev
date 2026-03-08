using OpenAI;
using OpenAI.Embeddings;

namespace root.Services
{
    public class EmbeddingService
    {
        private readonly OpenAIClient _client;

        public EmbeddingService(IConfiguration configuration)
        {
            var apikey = configuration["OpenAI:ApiKey"];
            _client = new OpenAIClient(apikey);
        }
        public async Task<float[]> CreateEmbeddingAsync(string text)
        {
            var response = await _client.GetEmbeddingClient("text-embedding-3-small").GenerateEmbeddingAsync(text);

            return response.Value.ToFloats().ToArray();
        }
    }
}

