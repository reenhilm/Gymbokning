namespace Gymbokning.Models.ViewModels
{
    public class IndexGymClassViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool bIsBooked { get; set; }
        public string Description { get; set; } = default!;
    }
}
