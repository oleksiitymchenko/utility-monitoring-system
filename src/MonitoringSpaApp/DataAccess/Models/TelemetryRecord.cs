using System;

namespace DAL.Models
{
    public class TelemetryRecord:AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid ControllerRegistryId { get; set; }
        public ControllerRegistry ControllerRegistry { get; set; }
        public string ImageUrl { get; set; }
        public long CounterValue { get; set; }
        public string BlobName { get; set; }
        public bool ProcessedSuccessful { get; set; }
    }
}
