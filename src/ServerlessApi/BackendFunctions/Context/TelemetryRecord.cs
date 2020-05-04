using System;
using System.ComponentModel.DataAnnotations;

namespace ServerlessApi.Context
{
    public class TelemetryRecord
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ControllerRegistryId { get; set; }
        public string ImageUrl { get; set; }
        public string BlobName { get; set; }
        public long CounterValue { get; set; }
        public bool ProcessedSuccessful { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
