using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Landing.Core.Models.Users
{
    /// <summary>
    /// Транзакция баллов пользователя
    /// </summary>
    public class UserPointsTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}