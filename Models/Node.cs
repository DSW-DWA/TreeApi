namespace TreeApi.Models
{
    public class Node
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long? ParentId { get; set; } // Optional parent node
        public Node Parent { get; set; }

        public long TreeId { get; set; }
        public Tree Tree { get; set; }

        // Children nodes
        public ICollection<Node> Children { get; set; }
    }

}
