namespace Landing.Core.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public ICollection<User> Participants { get; set; } = new List<User>();
        public bool IsOffline { get; set; } // Флаг очного мероприятия
        public string? Location { get; set; } // Адрес мероприятия
        public string? CustomHtmlTemplate { get; set; } // HTML-шаблон
        //для очного мероприятия:
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Address { get; set; }
        public ICollection<EventAttendance> Attendances { get; set; } = new HashSet<EventAttendance>();

    }
}
