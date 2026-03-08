using System.Text;
using System.Text.Json;

namespace root.Services
{
    public class LlmService
    {
        private readonly HttpClient _httpClient;
        public LlmService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AskAsync(string question, string context)
        {
            var prompt = $@" 
You are a codebase assistant.

Answer the question ONLY using the provided context.

If the answer is not in the context, say:
""The answer was not found in the repository.""

Context: 
{context},
Question: 
{question}

ANSWER:";

            var body = new
            {
                model = "tinyllama",
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(body);

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/generate",
                new StringContent(json, Encoding.UTF8, "application/json")
            );

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            using var doc = JsonDocument.Parse(content);

            if (doc.RootElement.TryGetProperty("response", out var responseProp))
            {
                return responseProp.GetString() ?? "";
            }

            if (doc.RootElement.TryGetProperty("error", out var errorProp))
            {
                return $"LLM Error: {errorProp.GetString()}";
            }

            return "Unexpected LLM response.";
        }
    }
}