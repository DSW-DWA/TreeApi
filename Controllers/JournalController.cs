using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeApi.Data;

namespace TreeApi.Controllers
{
    [ApiController]
    [Route("api.user.journal")]
    [Produces("application/json")]
    public class JournalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JournalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /api.user.journal.getRange
        [HttpPost("getRange")]
        public async Task<IActionResult> GetRange([FromQuery] int skip, [FromQuery] int take, [FromBody] JournalFilter filter)
        {
            var query = _context.ExceptionJournals.AsQueryable();

            if (filter.From.HasValue)
                query = query.Where(j => j.Timestamp >= filter.From);

            if (filter.To.HasValue)
                query = query.Where(j => j.Timestamp <= filter.To);

            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(j => j.ExceptionMessage.Contains(filter.Search) || j.StackTrace.Contains(filter.Search));

            var totalCount = await query.CountAsync();
            var items = await query.Skip(skip).Take(take).ToListAsync();

            return Ok(new
            {
                skip,
                count = totalCount,
                items = items.Select(j => new
                {
                    id = j.Id,
                    eventId = j.Id,
                    createdAt = j.Timestamp
                })
            });
        }

        // POST: /api.user.journal.getSingle
        [HttpPost("getSingle")]
        public async Task<IActionResult> GetSingle([FromQuery] Guid id)
        {
            var journal = await _context.ExceptionJournals.FindAsync(id);

            if (journal == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = journal.Id,
                eventId = journal.Id,
                createdAt = journal.Timestamp,
                text = journal.ExceptionMessage
            });
        }
    }

    public class JournalFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Search { get; set; }
    }
}
