namespace GameZone.Services.Services.GamesServices
{
    public interface IGamesServices
    {
        Task<IEnumerable<Game?>> GetAll();
        Task<Game?> GetById(int id);
        Task Create(CreateGameVM model);
        Task<Game?> Update(EditGameVM model);
        Task<Game?> Delete(int id);
    }
}
