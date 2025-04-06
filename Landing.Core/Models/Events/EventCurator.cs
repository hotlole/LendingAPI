namespace Landing.Core.Models.Events
{
    /// <summary>
    /// Представляет связь между мероприятием и его куратором.
    /// </summary>
    public class EventCurator
    {
        /// <summary>
        /// Уникальный идентификатор мероприятия.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Ссылка на объект мероприятия.
        /// </summary>
        public Event Event { get; set; } = null!;

        /// <summary>
        /// Уникальный идентификатор пользователя (куратора).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Ссылка на объект пользователя (куратора).
        /// </summary>
        public User User { get; set; } = null!;
    }
}
