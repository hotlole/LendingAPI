using System.ComponentModel.DataAnnotations;

namespace Landing.Core.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }  // Храним хэш пароля

        [Required]
        public string Role { get; set; } = "User";
    }
}
