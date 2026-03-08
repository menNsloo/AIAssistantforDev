
namespace root.Models
{
    public class VectorItem()
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; } = "";
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public string FileName { get; set; } = "";
    }
}
