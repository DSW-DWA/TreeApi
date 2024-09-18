using System.Xml.Linq;

namespace TreeApi.Models
{
    public class Tree
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Node> Nodes { get; set; }
    }
}
