﻿namespace Gymbokning.Models
{
    public class SeedUserDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
