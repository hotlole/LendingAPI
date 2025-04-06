namespace Landing.Core.Models.Events
{
    /// <summary>
    /// Представляет очное мероприятие с дополнительной информацией о местоположении.
    /// </summary>
    public class OfflineEvent : Event
    {
        /// <summary>
        /// Широта местоположения мероприятия.
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Долгота местоположения мероприятия.
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Адрес проведения мероприятия.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Кастомный HTML-шаблон для отображения информации о мероприятии.
        /// </summary>
        public string? CustomHtmlTemplate { get; set; }
    }
}
