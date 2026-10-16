
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/enrolments
        [HttpGet]
        public async Task<IActionResult> GetEnrolments()
        {
            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/enrolments/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrolment(int id)
        {
            var enrolment = await _context.Enrolments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrolmentID == id);

            if (enrolment == null)
            {
                return NotFound(new
                {
                    Message = "Enrolment not found."
                });
            }

            return Ok(enrolment);
        }

        // POST: api/enrolments
        [HttpPost]
        public async Task<IActionResult> CreateEnrolment(
            [FromBody] EnrolmentRequest request)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == request.EventID);

            if (!eventExists)
            {
                return BadRequest(new
                {
                    Message = "Event does not exist."
                });
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryID == request.CategoryID &&
                    c.EventID == request.EventID);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    Message = "Category does not belong to this event."
                });
            }

            var participantExists = await _context.Users
                .AnyAsync(u => u.UserID == request.ParticipantID);

            if (!participantExists)
            {
                return BadRequest(new
                {
                    Message = "Participant does not exist."
                });
            }

            var enrolment = new RaceDay.API.Models.Enrolment
            {
                ParticipantID = request.ParticipantID,
                EventID = request.EventID,
                CategoryID = request.CategoryID,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = enrolment.EnrolmentID },
                enrolment);
        }

        public class EnrolmentRequest
        {
            public int ParticipantID { get; set; }
            public int EventID { get; set; }
            public int CategoryID { get; set; }
        }

    }
}
