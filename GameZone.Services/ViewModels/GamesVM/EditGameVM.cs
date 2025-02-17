namespace GameZone.Services.ViewModels.GamesVM
{
    public class EditGameVM : CommonGameVM
    {
        public int Id { get; set; }

        public string? CurrentCover { get; set; } = string.Empty;

        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile? Cover { get; set; } = default!;
    }
}
