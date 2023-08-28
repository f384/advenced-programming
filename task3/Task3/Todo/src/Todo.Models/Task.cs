using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Todo.Models
{
    public class Task
    {
        public enum Priority
        {
            Low,
            Medium,
            High,
            VeryHigh
        }

        public int TaskId { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        [JsonIgnore]
        public virtual Category? Category{ get; set; } = null;      
        public int UserId { get; set; } = 0;
        [JsonIgnore]
        public virtual User? User{ get; set; } = null;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Priority Importance { get; set; } = Priority.Low;
        public DateTime? StartDate { get; set; } = null;
        public DateTime? DueDate { get; set; } = null;
        public bool IsCompleted { get; set; } = false;
    }
}
