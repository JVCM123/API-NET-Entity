using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/DevEvents")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventDbContext _dbContext;
        public DevEventsController(DevEventDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var devEvents = _dbContext.DevEvents.Where(e => !e.IsDeleted).ToList();
            return Ok(devEvents);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var devEvents = _dbContext.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(e => e.Id == id);

            if (devEvents == null)
            {
                return NotFound();
            }
            return Ok(devEvents);
        }

        [HttpPost]
        public IActionResult Post(DevEvent devEvent)
        {
            _dbContext.DevEvents.Add(devEvent);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEvent input)
        {
            var devEvents = _dbContext.DevEvents.SingleOrDefault(e => e.Id == id);
            if (devEvents == null)
            {
                return NotFound();
            }
            devEvents.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _dbContext.Update(devEvents);
            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var devEvents = _dbContext.DevEvents.SingleOrDefault(e => e.Id == id);
            if (devEvents == null)
            {
                return NotFound();
            }

            devEvents.Delete();

            _dbContext.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/speakers")]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker speaker)
        {
            speaker.DevEventId = id;

            var devEvents = _dbContext.DevEvents.Any(e => e.Id == id);

            if (!devEvents)
            {
                return NotFound();
            }

            _dbContext.devEventSpeakers.Add(speaker);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
