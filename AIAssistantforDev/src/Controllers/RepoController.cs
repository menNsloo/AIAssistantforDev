using Microsoft.AspNetCore.Mvc;
using root.Services;
using root.Models;

namespace root.Controllers
{
    [ApiController]
    [Route("api/repo")]
    public class RepoController : ControllerBase
    {
        private readonly GitHubRepoLoader _loader;
        private readonly CodeChunker _chunker;
        //private readonly EmbeddingService _embeddingService;
        private readonly LocalEmbeddingService _embeddingService;
        private readonly VectorStore _vectorStore;

        public RepoController(CodeChunker chunker, LocalEmbeddingService embeddingService, VectorStore vectorStore)
        {
            _loader = new GitHubRepoLoader();
            _chunker = chunker;
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
        }

        [HttpPost("process")]
        public async Task<IActionResult> LoadRepo([FromBody] string repoUrl)
        {
            var files = await _loader.LoadRepositoryAsync(repoUrl);
            var chunks = _chunker.ChunkFiles(files);


            foreach (var chunk in chunks)
            {
                var embeddding = await _embeddingService.CreateEmbeddingAsync(chunk.Content);
                _vectorStore.Add(new VectorItem
                {
                    Content = chunk.Content,
                    Embedding = embeddding,
                    FileName = chunk.FileName
                });
            }

            return Ok(new
            {
                FileCount = files.Count,
                ChunkCount = chunks.Count,
                VectorStore = _vectorStore.GetAll().Count,
                FirstVectorSize = _vectorStore.GetAll().FirstOrDefault()?.Embedding.Length
            });
        }
    }
}