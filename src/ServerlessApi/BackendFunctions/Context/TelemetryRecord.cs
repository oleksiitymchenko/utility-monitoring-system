using System;
using System.ComponentModel.DataAnnotations;

namespace ServerlessApi.Context
{
    public class TelemetryRecord
    {
        public Guid Id { get; set; }
        public Guid ControllerRegistryId { get; set; }
        public ControllerRegistry ControllerRegistry { get; set; }
        public string ImageUrl { get; set; }
        public string BlobName { get; set; }
        public long CounterValue { get; set; }
        public bool ProcessedSuccessful { get; set; }

        [MaxLength(256)]
        public string CreatedBy { get; set; }
        [MaxLength(256)]
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
