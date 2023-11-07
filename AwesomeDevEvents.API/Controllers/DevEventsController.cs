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


        /// <summary>
        /// Obter todos eventos cadastrados no banco
        /// </summary>
        /// <returns>Coleção de Eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var devEvents = _dbContext.DevEvents.Where(e => !e.IsDeleted).ToList();
            return Ok(devEvents);
        }

        /// <summary>
        /// Obter um evento com base no ID
        /// </summary>
        /// <param name="id">Identificador único do evento</param>
        /// <returns>Dados do evento</returns>
        /// <responde code="200">Sucesso</responde>
        /// <responde code="404">Não Encontrado</responde>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Cadastra um Evento no banco de dados
        /// </summary>
        /// <remarks>
        /// Exemplo de dados do Evento: {"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6","title": "string","description": "string","startDate": "2023-11-07T23:45:57.579Z","endDate": "2023-11-07T23:45:57.579Z","isDeleted": true}
        /// </remarks>
        /// <param name="devEvent">Dados do evento a ser inserido</param>
        /// <returns>Rota do objeto recém criado</returns>
        /// <response code="201">Sucesso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEvent devEvent)
        {
            _dbContext.DevEvents.Add(devEvent);
            _dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }


        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// <remarks>
        /// Exemplo de dados do Evento: {"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6","title": "string","description": "string","startDate": "2023-11-07T23:45:57.579Z","endDate": "2023-11-07T23:45:57.579Z","isDeleted": true}
        /// </remarks>
        /// <param name="id">Identificador único do evento</param>
        /// <param name="input">Novos dados do evento</param>
        /// <returns>null</returns>
        /// <response code="404">Evento não encontrado</response>
        /// <response code="204">Sucesso</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Excluir um evento
        /// </summary>
        /// <param name="id">Identificador único do evento</param>
        /// <returns>null</returns>
        /// <response code="404">Evento não encontrado</response>
        /// <response code="204">Sucesso</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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


        /// <summary>
        /// Cadastrar palestrante
        /// </summary>
        /// <remarks>
        /// Exemplo de dados do palestrante: {"id": "3fa85f64-5717-4562-b3fc-2c963f66afa6","name": "string","talkTitle": "string","talkDescription": "string","linkedInProfile": "string","devEventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"}</remarks>
        /// <param name="id">Identificador único do evento</param>
        /// <param name="speaker">Dados do palestrante a ser adicionado</param>
        /// <returns>null</returns>
        /// <response code="404">Evento não encontrado</response>
        /// <response code="204">Sucesso</response>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
