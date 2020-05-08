using DAL.Repositories.Interfaces;

namespace DAL
{
    public interface IUnitOfWork
    {
        IControllerRegistryRepository ControllerRegistryRepository { get; }
        ITelemetryRecordRepository TelemetryRecordRepository { get; }

        int SaveChanges();
    }
}
