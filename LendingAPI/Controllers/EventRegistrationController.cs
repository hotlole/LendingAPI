using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LendingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventRegistrationController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventRegistrationController(EventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Регистрация на мероприятие.
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <returns>Результат регистрации</returns>
        [HttpPost("{eventId}/register")]
        [Authorize]
        public async Task<IActionResult> Register(int eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || !int.TryParse(userId, out int userIdInt))
                return Unauthorized("Пользователь не найден.");

            try
            {
                var result = await _eventService.RegisterForEventAsync(eventId, userIdInt);
                if (result)
                    return Ok("Вы успешно записались на мероприятие.");
                else
                    return BadRequest("Вы уже записаны на это мероприятие.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
