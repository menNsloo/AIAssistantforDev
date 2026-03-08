using Microsoft.AspNetCore.Mvc;
using root.Services;

namespace root.Controllers
{
    [ApiController]
    [Route("api/ask")]
    public class AskController : ControllerBase
    {
        private readonly VectorSearchService _searchService;
        private readonly LocalEmbeddingService _embeddingService;
        private readonly VectorStore _vectorStore;
        private readonly LlmService _llmService;

        public AskController(
            VectorSearchService searchService,
            LocalEmbeddingService embeddingService,
            VectorStore vectorStore,
            LlmService llmService)
        {
            _searchService = searchService;
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
            _llmService = llmService;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] string question)
        {
            var queryEmbedding = await _embeddingService.CreateEmbeddingAsync(question);
            Console.WriteLine(_vectorStore.GetAll().Count);
            Console.WriteLine($"Vector count: {_vectorStore.GetAll().Count}");

            var results = _searchService.Search(
                queryEmbedding,
                _vectorStore.GetAll(),
                5
            );

            var context = string.Join("\n\n",
                results.Select(r => $"FILE: {r.FileName}\n{r.Content}"));

            var answer = await _llmService.AskAsync(question, context);

            var uniqueSources = results
                .Select(r => r.FileName)
                .Distinct()
                .ToList();

            return Ok(new
            {
                Question = question,
                answer,
                Sources = uniqueSources
            });
        }
    }
}