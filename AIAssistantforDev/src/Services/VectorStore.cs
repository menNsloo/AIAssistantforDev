using root.Models;

namespace root.Services
{
    public class VectorStore
    {
        private readonly List<VectorItem> _items = new();

        public void Add(VectorItem item)
        {
            _items.Add(item);
        }

        public List<VectorItem> GetAll()
        {
            return _items;
        }
    }
}