
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/results
        [HttpGet]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.Results
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/results/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _context.Results
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Result not found."
                });
            }

            return Ok(result);
        }

        // POST: api/results
        [HttpPost]
        public async Task<IActionResult> CreateResult(
            [FromBody] ResultRequest request)
        {
            if (request.FinishPosition <= 0)
            {
                return BadRequest(new
                {
                    Message = "Finish position must be greater than zero."
                });
            }

            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentID == request.EnrolmentID);

            if (!enrolmentExists)
            {
                return BadRequest(new
                {
                    Message = "Enrolment does not exist."
                });
            }

            var resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentID == request.EnrolmentID);

            if (resultExists)
            {
                return Conflict(new
                {
                    Message = "A result already exists for this enrolment."
                });
            }

            var result = new RaceDay.API.Models.Result
            {
                EnrolmentID = request.EnrolmentID,
                FinishTime = request.FinishTime,
                FinishPosition = request.FinishPosition,
                ResultStatus = "Recorded"
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultID },
                result);
        }

        public class ResultRequest
        {
            public int EnrolmentID { get; set; }
            public TimeSpan FinishTime { get; set; }
            public int FinishPosition { get; set; }
        }
    }
}
