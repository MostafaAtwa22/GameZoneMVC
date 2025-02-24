using GameZone.Core.Models;

namespace GameZone.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }

        IGenericRepository<Game> Games { get; }

        IGenericRepository<Device> Devices { get; }

        IGenericRepository<ApplicationUser> ApplicationUsers { get; }

        Task<int> Complete();
    }
}
