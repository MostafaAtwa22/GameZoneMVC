namespace GameZone.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }

        IGenericRepository<Game> Games { get; }

        IGenericRepository<Device> Devices { get; }

        Task<int> Complete();
    }
}
