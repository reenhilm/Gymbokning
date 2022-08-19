namespace Gymbokning.Models
{
    public class ApplicationUserGymClass
    {
        public int Id { get; set; }

        //Foreign key
        public int GymClassId { get; set; }

        //Foreign key
        public string ApplicationUserId { get; set; }

        //Nav prop
        public ApplicationUser ApplicationUser { get; set; }
        public GymClass GymClass { get; set; }
    }
}
