using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeApi.Data;
using TreeApi.Models;

namespace TreeApi.Controllers
{
    [ApiController]
    [Route("api.user.tree")]
    [Produces("application/json")]
    public class TreeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TreeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api.user.tree.get
        [HttpPost("get")]
        public async Task<IActionResult> GetTree([FromQuery] string treeName)
        {
            var tree = await _context.Trees.Include(t => t.Nodes).FirstOrDefaultAsync(t => t.Name == treeName);

            if (tree == null)
            {
                // Automatically create a tree if not exists
                tree = new Tree { Name = treeName };
                _context.Trees.Add(tree);
                await _context.SaveChangesAsync();
            }

            return Ok(tree);
        }
    }

    [ApiController]
    [Route("api.user.tree.node")]
    [Produces("application/json")]
    public class TreeNodeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TreeNodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api.user.tree.node.create
        [HttpPost("create")]
        public async Task<IActionResult> CreateNode([FromQuery] string treeName, [FromQuery] long parentNodeId, [FromQuery] string nodeName)
        {
            var tree = await _context.Trees.Include(t => t.Nodes).FirstOrDefaultAsync(t => t.Name == treeName);
            if (tree == null) return NotFound("Tree not found");

            var parentNode = tree.Nodes.FirstOrDefault(n => n.Id == parentNodeId);
            if (parentNode == null) return BadRequest("Parent node not found");

            if (parentNode.Children.Any(c => c.Name == nodeName))
            {
                return BadRequest("Node name must be unique across siblings");
            }

            var newNode = new Node { Name = nodeName, Parent = parentNode };
            _context.Nodes.Add(newNode);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: /api.user.tree.node.delete
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteNode([FromQuery] string treeName, [FromQuery] long nodeId)
        {
            var tree = await _context.Trees.Include(t => t.Nodes).FirstOrDefaultAsync(t => t.Name == treeName);
            if (tree == null) return NotFound("Tree not found");

            var node = tree.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null) return BadRequest("Node not found");

            _context.Nodes.Remove(node);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: /api.user.tree.node.rename
        [HttpPost("rename")]
        public async Task<IActionResult> RenameNode([FromQuery] string treeName, [FromQuery] long nodeId, [FromQuery] string newNodeName)
        {
            var tree = await _context.Trees.Include(t => t.Nodes).FirstOrDefaultAsync(t => t.Name == treeName);
            if (tree == null) return NotFound("Tree not found");

            var node = tree.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null) return BadRequest("Node not found");

            if (node.Parent.Children.Any(c => c.Name == newNodeName))
            {
                return BadRequest("Node name must be unique across siblings");
            }

            node.Name = newNodeName;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
