using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServerlessApi.Context
{
    public class ControllerRegistry
    {
        public Guid Id { get; set; }
        public string ApplicationUserId { get; set; }
       // public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int CounterType { get; set; }

        [MaxLength(256)]
        public string CreatedBy { get; set; }
        [MaxLength(256)]
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<TelemetryRecord> TelemetryRecord { get; set; }
    }
}
