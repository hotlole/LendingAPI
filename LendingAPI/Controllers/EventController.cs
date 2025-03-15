using Landing.Application.Interfaces;
using Landing.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Landing.API.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        // Получить все мероприятия
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventRepository.GetAllAsync();
            return Ok(events);
        }

        // Получить мероприятие по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null) return NotFound("Мероприятие не найдено.");
            return Ok(eventItem);
        }

        // Создать новое мероприятие
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Event eventItem)
        {
            if (eventItem == null) return BadRequest("Некорректные данные.");

            var createdEvent = await _eventRepository.CreateAsync(eventItem);
            return CreatedAtAction(nameof(GetById), new { id = createdEvent.Id }, createdEvent);
        }

        // Обновить мероприятие
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Event eventItem)
        {
            if (eventItem == null || id != eventItem.Id) return BadRequest("Некорректные данные.");

            var updatedEvent = await _eventRepository.UpdateAsync(eventItem);
            return Ok(updatedEvent);
        }

        // Удалить мероприятие
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _eventRepository.DeleteAsync(id);
            if (!success) return NotFound("Мероприятие не найдено.");

            return NoContent();
        }
    }
}
