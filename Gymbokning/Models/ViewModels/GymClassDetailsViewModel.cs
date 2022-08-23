using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models.ViewModels
{
    public class GymClassDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime { get { return StartTime + Duration; } }
        public string Description { get; set; } = default!;
        [Display(Name = "Attending members")]
        public IEnumerable<string> AttendingApplicationUserEmails { get; set; } = new List<string>();
    }
}
