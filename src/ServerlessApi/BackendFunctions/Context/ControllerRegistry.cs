using System;
using System.ComponentModel.DataAnnotations;

namespace ServerlessApi.Context
{
    public class ControllerRegistry
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CounterType { get; set; } //TODO: Add enum
        public DateTime CreatedOn { get; set; }
    }
}
