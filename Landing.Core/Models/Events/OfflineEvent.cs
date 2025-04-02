namespace Landing.Core.Models.Events
{
    public class OfflineEvent : Event
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? CustomHtmlTemplate { get; set; }
    }
}
