namespace GameZone.Services.Services.CategoriesServices
{
    public interface ICategoriesServices
    {
        Task<List<SelectListItem>> CategoriesList();
    }
}
