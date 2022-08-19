namespace Gymbokning.Models
{
    public class ApplicationUserGymClass
    {
        //We could have an Id but we don't need an Id. New requirement says we should not have an Id
        //public int Id { get; set; }

        //Foreign key
        public int GymClassId { get; set; }

        //Foreign key
        public string ApplicationUserId { get; set; }

        //Nav prop
        public ApplicationUser ApplicationUser { get; set; }
        public GymClass GymClass { get; set; }
    }
}
