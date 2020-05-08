﻿using DAL.Core;
using System;
using System.Collections.Generic;

namespace DAL.Models
{
    public class ControllerRegistry:AuditableEntity
    {
        public Guid Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CounterType CounterType { get; set; }

        public virtual ICollection<TelemetryRecord> TelemetryRecord { get; set; }
    }
}
