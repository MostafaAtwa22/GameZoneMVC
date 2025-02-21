namespace GameZone.Services.ViewModels.AccountVM
{
    public class ForgetPasswordVM
    {
        [DataType(DataType.EmailAddress)]    
        [Required(ErrorMessage ="The Email is required")]
        public string Email { get; set; } = string.Empty;
    }
}
