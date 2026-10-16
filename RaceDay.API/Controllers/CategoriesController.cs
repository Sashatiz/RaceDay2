
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API;

namespace RaceDay.API.Controllers
{
    [Route("api/events/{eventId}/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/1/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == eventId);

            if (!eventExists)
            {
                return NotFound(new
                {
                    Message = "Event not found."
                });
            }

            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return Ok(categories);
        }
    }
}
