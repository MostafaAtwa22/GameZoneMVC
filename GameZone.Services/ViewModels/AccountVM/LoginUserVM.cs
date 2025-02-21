namespace GameZone.Services.ViewModels.AccountVM
{
    public class LoginUserVM
    {
        [Required]
        [Display(Name = "Username Or Email")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
