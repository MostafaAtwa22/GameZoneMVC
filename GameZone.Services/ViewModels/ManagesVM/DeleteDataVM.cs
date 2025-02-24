namespace GameZone.Services.ViewModels.ManagesVM
{
    public class DeleteDataVM
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
    }
}
