namespace GameZone.Services.ViewModels.GamesVM
{
    public class CreateGameVM : CommonGameVM
    {
        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile Cover { get; set; } = default!;
    }
}
