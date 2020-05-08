using Microsoft.Extensions.Logging;

namespace MonitoringSpa.Helpers
{
    public static class LoggingEvents
    {
        public static readonly EventId INIT_DATABASE = new EventId(101, "Error whilst creating and seeding database");
    }

}
