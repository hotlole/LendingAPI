namespace Landing.Core.Models.Events
{
    public enum EventType
    {
        Regular,    // Обычное мероприятие
        Curated,    // Курируемое мероприятие
        Offline     // Очное мероприятие
    }

    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public EventType Type { get; set; }  // Тип мероприятия
        public ICollection<EventAttendance> Attendances { get; set; } = new List<EventAttendance>();

    }

}
