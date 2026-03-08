using Models.CodeChunk;
using Models.CodeFile;

namespace root.Services
{
    public class CodeChunker
    {
        public List<CodeChunk> ChunkFiles(List<CodeFile> files, int chunkSize = 1000)
        {
            var chunks = new List<CodeChunk>();

            foreach (var file in files)
            {
                var content = file.Content;
                if (string.IsNullOrWhiteSpace(content))
                    continue;

                for (int i = 0; i < content.Length; i += chunkSize)
                {
                    var length = Math.Min(chunkSize, content.Length - i);

                    var chunkText = content.Substring(i, length);

                    chunks.Add(new CodeChunk
                    {
                        Content = chunkText,
                        FileName = file.FileName,
                        FilePath = file.FilePath
                    });
                }
            }

            return chunks;
        }
    }
}