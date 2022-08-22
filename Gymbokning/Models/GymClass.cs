namespace Gymbokning.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime { get { return StartTime + Duration; } }
        public string Description { get; set; } = null!;
        //Nav prop
        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
        public ICollection<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; } = new List<ApplicationUserGymClass>();
    }
}
