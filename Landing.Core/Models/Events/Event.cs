using System.ComponentModel.DataAnnotations;

namespace Landing.Core.Models.Events
{
    /// <summary>
    /// Тип мероприятия.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Базовый тип мероприятия.
        /// </summary>
        Base,

        /// <summary>
        /// Обычное мероприятие.
        /// </summary>
        Regular,    // Обычное мероприятие

        /// <summary>
        /// Курируемое мероприятие.
        /// </summary>
        Curated,    // Курируемое мероприятие

        /// <summary>
        /// Очное мероприятие.
        /// </summary>
        Offline     // Очное мероприятие
    }

    /// <summary>
    /// Представляет событие в системе.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Уникальный идентификатор мероприятия.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название мероприятия.
        /// </summary>
        /// <example>Конференция по технологиям</example>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание мероприятия.
        /// </summary>
        /// <example>Мероприятие, посвященное последним тенденциям в мире технологий.</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Дата проведения мероприятия.
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /// <summary>
        /// Путь к изображению мероприятия.
        /// </summary>
        /// <example>/images/tech-conference.jpg</example>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Тип мероприятия.
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// Список записавшихся на мероприятие.
        /// </summary>
        public ICollection<EventAttendance> Attendances { get; set; } = new List<EventAttendance>();
    }
}
