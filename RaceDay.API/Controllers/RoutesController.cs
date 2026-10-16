
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API;

namespace RaceDay.API.Controllers
{
    [Route("api/events/{eventId}/route")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public RoutesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/1/route
        [HttpGet]
        public async Task<IActionResult> GetRoute(int eventId)
        {
            var route = await _context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.EventID == eventId);

            if (route == null)
            {
                return NotFound(new
                {
                    Message = "Route not found for this event."
                });
            }

            return Ok(route);
        }
    }
}
