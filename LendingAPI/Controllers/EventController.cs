using Landing.Application.Interfaces;
using Landing.Core.Models;
using Landing.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

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
        private readonly IMapper _mapper;

        public EventController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить список всех мероприятий.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventRepository.GetAllAsync();
            var eventDtos = _mapper.Map<IEnumerable<EventDto>>(events);
            return Ok(eventDtos);
        }

        /// <summary>
        /// Получить мероприятие по ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);
            if (eventItem == null) return NotFound("Мероприятие не найдено.");

            var eventDto = _mapper.Map<EventDto>(eventItem);
            return Ok(eventDto);
        }

        /// <summary>
        /// Создать новое мероприятие (только для Админов).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EventDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var eventItem = _mapper.Map<Event>(dto);
            await _eventRepository.CreateAsync(eventItem);
            var eventDto = _mapper.Map<EventDto>(eventItem);

            return CreatedAtAction(nameof(GetById), new { id = eventItem.Id }, eventDto);
        }

        /// <summary>
        /// Обновить мероприятие (только для Админов).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(EventDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            if (!ModelState.IsValid || id != dto.Id) return BadRequest("Некорректные данные.");

            var existingEvent = await _eventRepository.GetByIdAsync(id);
            if (existingEvent == null) return NotFound("Мероприятие не найдено.");

            _mapper.Map(dto, existingEvent);
            await _eventRepository.UpdateAsync(existingEvent);

            var eventDto = _mapper.Map<EventDto>(existingEvent);
            return Ok(eventDto);
        }

        /// <summary>
        /// Удалить мероприятие (только для Админов).
        /// </summary>
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