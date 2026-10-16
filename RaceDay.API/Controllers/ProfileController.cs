
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/profile/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.UserID == id)
                .Select(u => new
                {
                    u.UserID,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.ProfilePictureUrl,
                    u.RoleID
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new
                {
                    Message = "User not found."
                });
            }

            return Ok(user);
        }
    }
}
