using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GameZone.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string? Cover { get; set; } = string.Empty;
    }
}
