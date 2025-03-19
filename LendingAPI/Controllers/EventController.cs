using Landing.Application.Interfaces;
using Landing.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Landing.API.Controllers
{
    /// <summary>
    /// Контроллер для управления мероприятиями.
    /// </summary>
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        /// <summary>
        /// Конструктор контроллера.
        /// </summary>
        /// <param name="eventRepository">Репозиторий мероприятий.</param>
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        /// <summary>
        /// Получить список всех мероприятий.
        /// </summary>
        /// <returns>Список мероприятий.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Event>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventRepository.GetAllAsync();
            return Ok(events);
        }

        /// <summary>
        /// Получить мероприятие по ID.
        /// </summary>
        /// <param name="id">ID мероприятия.</param>
        /// <returns>Мероприятие с указанным ID.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Event), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null) return NotFound("Мероприятие не найдено.");
            return Ok(eventItem);
        }

        /// <summary>
        /// Создать новое мероприятие (только для Админов).
        /// </summary>
        /// <param name="eventItem">Данные мероприятия.</param>
        /// <returns>Созданное мероприятие.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Event), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Event eventItem)
        {
            if (eventItem == null) return BadRequest("Некорректные данные.");

            var createdEvent = await _eventRepository.CreateAsync(eventItem);
            return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
        }


        /// <summary>
        /// Обновить мероприятие (только для Админов).
        /// </summary>
        /// <param name="id">ID мероприятия.</param>
        /// <param name="eventItem">Обновленные данные мероприятия.</param>
        /// <returns>Обновленное мероприятие.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Event), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, [FromBody] Event eventItem)
        {
            if (eventItem == null || id != eventItem.Id) return BadRequest("Некорректные данные.");

            var updatedEvent = await _eventRepository.UpdateAsync(eventItem);
            return Ok(updatedEvent);
        }
        /// <summary>
        /// Удалить мероприятие (только для Админов).
        /// </summary>
        /// <param name="id">ID мероприятия.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventRepository.DeleteAsync(id);
            if (!success) return NotFound("Мероприятие не найдено.");

            return NoContent();
        }
    }
}
