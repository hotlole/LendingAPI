using System.ComponentModel.DataAnnotations;

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
      
        public string? RefreshToken { get; set; } // 🔹 Сам токен
        public DateTime? RefreshTokenExpiryTime { get; set; } // 🔹 Срок действия токена
    }
}
