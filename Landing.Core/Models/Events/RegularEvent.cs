using Landing.Core.Models.Users;

namespace Landing.Core.Models.Events
{
    public class RegularEvent : Event
    {
        public ICollection<User> Participants { get; set; } = new List<User>();

    }
}
