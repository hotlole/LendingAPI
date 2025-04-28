namespace Landing.Application.DTOs.Events
{
    public class RegisteredUserDto
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Фамилия Имя Отчество пользователя.
        /// </summary>
        public string FullName { get; set; }
    }
}
