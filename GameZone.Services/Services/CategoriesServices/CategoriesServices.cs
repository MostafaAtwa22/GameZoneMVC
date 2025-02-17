namespace GameZone.Services.Services.CategoriesServices
{
    public class CategoriesServices : ICategoriesServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SelectListItem>> CategoriesList()
        {
            var categories = await _unitOfWork
                .Categories
                .GetAll();

            return categories
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();
        }
    }
}
