namespace GameZone.Services.Services.DevicesServices
{
    public interface IDevicesServices
    {
        Task<List<SelectListItem>> DevicesSelectList();
    }
}
