using DAL.Repositories;
using DAL.Repositories.Interfaces;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IControllerRegistryRepository controllerRegistryRepository;
        private ITelemetryRecordRepository telemetryRecordRepository;

        public IControllerRegistryRepository ControllerRegistryRepository 
        {
            get
            {
                if (controllerRegistryRepository == default)
                    controllerRegistryRepository = new ControllerRegistryRepository(_context);
                
                return controllerRegistryRepository;
            }
        }

        public ITelemetryRecordRepository TelemetryRecordRepository 
        {
            get
            {
                if (telemetryRecordRepository == default)
                    telemetryRecordRepository = new TelemetryRecordRepository(_context);

                return telemetryRecordRepository;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
