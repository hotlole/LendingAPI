namespace Landing.Core.Models.Users
{
    public class EmailConfirmationToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsUsed { get; set; } = false;

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
