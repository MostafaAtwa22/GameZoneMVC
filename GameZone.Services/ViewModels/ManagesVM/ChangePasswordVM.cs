namespace GameZone.Services.ViewModels.ManagesVM
{
    public class ChangePasswordVM
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string oldPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
           ErrorMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The Password does not match")]
        [Display(Name = "Confirm new Password")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
