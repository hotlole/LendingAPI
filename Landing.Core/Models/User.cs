using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Landing.Core.Models.Events;

namespace Landing.Core.Models
{
    /// <summary>
    /// Пользователь системы
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя
        /// </summary>
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; } = false;
        public string PasswordHash { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public ICollection<EventAttendance> Attendances { get; set; } = new HashSet<EventAttendance>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<UserPointsTransaction> PointsTransactions { get; set; } = new List<UserPointsTransaction>();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Points { get; set; }
    }
}
