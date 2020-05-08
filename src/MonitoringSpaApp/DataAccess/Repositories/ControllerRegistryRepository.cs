using DAL.Models;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories
{
    public class ControllerRegistryRepository : Repository<ControllerRegistry>, IControllerRegistryRepository
    {
        public ControllerRegistryRepository(ApplicationDbContext context) : base(context)
        { }
    }
}
