namespace GameZone.Services.ViewModels.ManagesVM
{
    public class EmailVM
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
