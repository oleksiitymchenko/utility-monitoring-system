using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class TelemetryRecordRepository : Repository<TelemetryRecord>, ITelemetryRecordRepository
    {
        public TelemetryRecordRepository(DbContext context) : base(context)
        {
        }
    }
}
