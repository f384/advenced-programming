using System;
using System.Collections.Generic;
using University.Interfaces;

namespace University.Models
{
    public class Exam : IEntity
    {
        public string ExamId { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public DateTime? Date { get; set; } = null;
        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Professor { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = false;

        public string GetKey()
        {
            return ExamId;
        }
    }
}
