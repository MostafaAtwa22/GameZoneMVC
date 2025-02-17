namespace GameZone.Services.Services.DevicesServices
{
    public class DevicesServices : IDevicesServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public DevicesServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SelectListItem>> DevicesSelectList()
        {
            var categories = await _unitOfWork
                .Devices
                .GetAll();

            return categories
                .OrderBy(d => d.Name)
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Name
                })
                .ToList();
        }
    }
}
