namespace GameZone.Services.ViewModels.RolesVM
{
    public class CreateRoleVM
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
