using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace GameZone.Repository.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Categories = new GenericRepository<Category>(_context);
            Games = new GenericRepository<Game>(_context);
            Devices = new GenericRepository<Device>(_context); 
        }

        public IGenericRepository<Category> Categories { get; private set; }

        public IGenericRepository<Game> Games { get; private set; }

        public IGenericRepository<Device> Devices { get; private set; }

        public async Task<int> Complete()
            => await _context.SaveChangesAsync();
        
        public void Dispose()
            => _context.Dispose();
    }
}
