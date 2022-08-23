using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "First Name")]
        string FirstName { get; set; } = null!;
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Last Name")]
        string LastName { get; set; } = null!;
        [Display(Name = "Full Name")]
        string FullName => $"{FirstName} {LastName}";

        //Nav prop
        public ICollection<GymClass> GymClasses { get; set; } = new List<GymClass>();
        public ICollection<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; } = new List<ApplicationUserGymClass>();
    }
}
