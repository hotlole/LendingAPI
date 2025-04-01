using System.ComponentModel.DataAnnotations;

namespace Landing.Core.Models
{
    /// <summary>
    /// Явка на мероприятие
    /// </summary>
    public class EventAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EventId { get; set; }

        public bool IsConfirmed { get; set; } = false;
        public int PointsAwarded { get; set; } = 0;

        public DateTime ConfirmedAt { get; set; }

        public User User { get; set; } = null!;
        public Event Event { get; set; } = null!;
    }
}
