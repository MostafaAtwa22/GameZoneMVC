using Org.BouncyCastle.Bcpg;

namespace GameZone.Services.ViewModels.AccountVM
{
    public class RegisterUserMV
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Username can only contain letters, digits, and underscores with no spaces.")]
        public string UserName { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The Password does not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string? RoleId { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> RolesList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
