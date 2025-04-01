using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Landing.Core.Models
{
    /// <summary>
    /// Явка на мероприятие
    /// </summary>
    [SwaggerSchema("Модель, описывающая явку пользователя на мероприятие.")]
    public class EventAttendance
    {
        /// <summary>
        /// Идентификатор записи о явке.
        /// </summary>
        [Key]
        [SwaggerSchema("Идентификатор записи о явке на мероприятие.")]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        [Required]
        [SwaggerSchema("Идентификатор пользователя, подтверждающего явку.")]
        public int UserId { get; set; }

        /// <summary>
        /// Идентификатор мероприятия.
        /// </summary>
        [Required]
        [SwaggerSchema("Идентификатор мероприятия, на которое подтверждена явка.")]
        public int EventId { get; set; }

        /// <summary>
        /// Подтверждена ли явка.
        /// </summary>
        [SwaggerSchema("Статус подтверждения явки (true — подтверждено, false — не подтверждено).")]
        public bool IsConfirmed { get; set; } = false;

        /// <summary>
        /// Количество баллов, начисленных пользователю за явку.
        /// </summary>
        [SwaggerSchema("Количество баллов, начисленных за подтверждённую явку на мероприятие.")]
        public int PointsAwarded { get; set; } = 0;

        /// <summary>
        /// Дата и время подтверждения явки.
        /// </summary>
        [SwaggerSchema("Дата и время, когда была подтверждена явка.")]
        public DateTime ConfirmedAt { get; set; }

        /// <summary>
        /// Пользователь, связанный с явкой.
        /// </summary>
        [SwaggerSchema("Пользователь, связанный с данной записью о явке.")]
        public User User { get; set; } = null!;

        /// <summary>
        /// Мероприятие, для которого была подтверждена явка.
        /// </summary>
        [SwaggerSchema("Мероприятие, для которого была подтверждена явка.")]
        public Event Event { get; set; } = null!;
    }
}
