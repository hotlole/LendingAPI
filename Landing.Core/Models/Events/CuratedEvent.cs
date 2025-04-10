using Landing.Core.Models.Users;

namespace Landing.Core.Models.Events
{
    public class CuratedEvent : Event
    {
        public  ICollection<User> Curators { get; set; } = new List<User>();

    }
}

