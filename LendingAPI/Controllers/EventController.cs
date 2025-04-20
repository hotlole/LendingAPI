using Landing.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Swashbuckle.AspNetCore.Annotations;
using Landing.Core.Models.Events;
using Landing.Application.DTOs.Events;

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
        private readonly EventService _eventService;

        public EventController(IEventRepository eventRepository, IMapper mapper, EventService eventService)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _eventService = eventService;
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

            var eventItem = await _eventService.CreateAsync(dto);
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

        /// <summary>
        /// Подтвердить явку пользователя на мероприятие и начислить баллы (только для Админов и Модераторов).
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="points">Количество баллов для начисления.</param>
        /// <returns>Статус подтверждения явки.</returns>
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost("{eventId}/confirm/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerOperation(Summary = "Подтвердить явку пользователя на мероприятие")]
        public async Task<IActionResult> ConfirmAttendance(int eventId, int userId, [FromBody] int points)
        {
            try
            {
                await _eventService.ConfirmAttendanceAsync(userId, eventId, points);
                return Ok("Явка подтверждена");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Обновить HTML-шаблон мероприятия.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="htmlTemplate">Новый HTML-шаблон.</param>
        /// <returns>Статус обновления шаблона.</returns>
        [HttpPost("{eventId}/template")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [SwaggerOperation(Summary = "Обновить HTML-шаблон мероприятия")]
        public async Task<IActionResult> UpdateHtmlTemplate(int eventId, [FromBody] string htmlTemplate)
        {
            try
            {
                await _eventService.UpdateHtmlTemplateAsync(eventId, htmlTemplate);
                return Ok("Шаблон успешно обновлён.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получить HTML-шаблон мероприятия.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <returns>HTML-шаблон мероприятия.</returns>
        [HttpGet("{eventId}/template")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Получить HTML-шаблон мероприятия")]
        public async Task<IActionResult> GetHtmlTemplate(int eventId)
        {
            var template = await _eventService.GetHtmlTemplateAsync(eventId);
            if (template == null)
                return NotFound("Шаблон не найден.");

            return Ok(template);
        }
        /// <summary>
        /// Назначает пользователя куратором мероприятия.
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия.</param>
        /// <param name="userId">Идентификатор пользователя, которого нужно назначить куратором.</param>
        /// <response code="200">Куратор успешно назначен на мероприятие.</response>
        /// <response code="400">Ошибка: если мероприятие или пользователь не найдены, или если произошла другая ошибка.</response>
        [HttpPost("assign-curator")]
        [SwaggerOperation(
            Summary = "Назначить пользователя куратором мероприятия",
            Description = "Позволяет назначить пользователя куратором мероприятия по его ID."
        )]
        [SwaggerResponse(200, "Куратор успешно назначен на мероприятие.")]
        [SwaggerResponse(400, "Ошибка: мероприятие или пользователь не найдены, или ошибка при выполнении операции.")]
        public async Task<IActionResult> AssignCurator([FromQuery] int eventId, [FromQuery] int userId)
        {
            try
            {
                await _eventService.AssignCuratorAsync(eventId, userId);
                return Ok("Куратор успешно назначен на мероприятие.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка: {ex.Message}");
            }
        }

    }
}